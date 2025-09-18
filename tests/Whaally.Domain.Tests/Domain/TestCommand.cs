using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Domain;

[Immutable, GenerateSerializer]
internal record TestCommand : ICommand
{
    public string? AggregateId { get; init; }
    public IEnumerable<IEvent> Events { get; init; } = new IEvent[] { };
    public Result Result { get; init; } = Result.Ok();
}

internal class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
{
    public ICommandResult Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
    {
        var result = Command.Ok();
        
        foreach (var @event in command.Events)
        {
            result.Stage(@event);
        }

        return result.WithReasons(command.Result.Reasons);
    }
}