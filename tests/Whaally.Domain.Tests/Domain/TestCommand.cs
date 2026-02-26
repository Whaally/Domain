using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Domain;

[Immutable, GenerateSerializer]
internal record TestCommand : ICommand
{
    [Id(0)]
    public string? AggregateId { get; init; }
    [Id(1)]
    public IEnumerable<IEvent> Events { get; init; } = new IEvent[] { };
    [Id(2)]
    public Result Result { get; init; } = Result.Ok();
}

internal class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
{
    public ICommandResult Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
    {
        var result = new Command();
        
        foreach (var @event in command.Events)
        {
            result.Stage(@event);
        }

        return result.WithReasons(command.Result.Reasons);
    }
}