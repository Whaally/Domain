using System.Diagnostics;
using System.Security.Cryptography;
using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using FluentAssertions;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Xunit.Abstractions;

namespace Skyhop.Domain.Tests;

public class GraphGenerator
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GraphGenerator(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task GenerateGraph()
    {
        var graph = new DotGraph()
            .WithIdentifier("Aggregates");

        var ag = FlightMetadata.Instance;

        var agNode = new DotNode()
            .WithIdentifier($"{ag.Namespace}.{ag.Name}")
            .WithLabel($"<b>Aggregate:</b> {ag.Name}", true)
            .WithColor("#B3CEE5")
            .WithShape(DotNodeShape.Box3D)
            .WithStyle(DotNodeStyle.Filled);

        graph.Add(agNode);
        
        foreach (var cmd in ag.Commands)
        {
            var identifier = $"{cmd.Namespace}.{cmd.Name}";
            
            var node = new DotNode()
                .WithIdentifier(identifier)
                .WithShape(DotNodeShape.Box)
                .WithLabel($"<b>Command:</b> {cmd.Name}", true)
                .WithColor("#90D5FF")
                .WithStyle(DotNodeStyle.Filled);

            graph.Add(node);

            graph.Add(new DotEdge()
                .From(agNode)
                .To(node)
                .WithAttribute("dir", "forward")
                .WithLabel("Reads"));

            foreach (var invokedCmd in cmd.Invoked)
            {
                graph.Add(new DotEdge()
                    .From(identifier)
                    .To($"{invokedCmd.Namespace}.{invokedCmd.Name}")
                    .WithAttribute("dir", "forward")
                    .WithLabel("Invoke"));
            }

            foreach (var stagedEvent in cmd.Staged)
            {
                graph.Add(new DotEdge()
                    .From(identifier)
                    .To($"{stagedEvent.Namespace}.{stagedEvent.Name}")
                    .WithAttribute("dir", "forward")
                    .WithLabel("Stage"));
            }
        }

        foreach (var @event in ag.Events)
        {
            var node = new DotNode()
                .WithIdentifier($"{@event.Namespace}.{@event.Name}")
                .WithShape(DotNodeShape.Box)
                .WithLabel($"<b>Event:</b> {@event.Name}", true)
                .WithColor("#FFB343")
                .WithStyle(DotNodeStyle.Filled);

            graph.Add(node);
            
            graph.Add(new DotEdge()
                .From(node)
                .To(agNode)
                .WithAttribute("dir", "forward")
                .WithLabel("Writes"));
        }

        await using var writer = new StringWriter();
        var context = new CompilationContext(writer, new CompilationOptions());
        await graph.CompileAsync(context);

        var result = writer.GetStringBuilder().ToString();

        _testOutputHelper.WriteLine(result);
    }
}