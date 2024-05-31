using BenchmarkDotNet.Attributes;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Aggregate;
using Whaally.Domain.Command;

namespace Skyhop.Domain.Benchmark.Aggregate;

[ShortRunJob]
public class EvaluateCommandBenchmark
{
    public class Aggregate : IAggregate { }
    public class Command : ICommand { }

    public class CommandHandler : ICommandHandler<Aggregate, Command>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Aggregate> context, Command @event)
            => Result.Ok();
    }

    private IServiceProvider _services;
    private DefaultAggregateHandler<Aggregate>? _handler;
    
    public EvaluateCommandBenchmark()
    {
        _services = new ServiceCollection()
            .AddSingleton<IAggregateHandlerFactory, DefaultAggregateHandlerFactory>()
            // .AddTransient<IEvaluationAgent, DefaultEvaluationAgent>()
            .AddTransient<ICommandHandler<Aggregate, Command>, CommandHandler>()
            .BuildServiceProvider();
    }
    
    [GlobalSetup]
    public void Setup()
    {
        _handler = new DefaultAggregateHandler<Aggregate>(_services, "");
    }

    [Benchmark]
    public async Task Apply() => await _handler!.Evaluate(new CommandEnvelope(new Command(), new CommandMetadata()));
}