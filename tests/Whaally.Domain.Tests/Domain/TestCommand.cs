using FluentResults;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;

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
    public IResultBase Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
    {
        foreach (var @event in command.Events)
        {
            context.StageEvent(@event.GetType(), @event);
        }

        return command.Result;
    }
}