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
    public Result Result { get; init; } = Result.Success();
}

internal class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
{
    public async Task<ICommandResult> Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
    {
        var result = Command.Result;
        
        foreach (var @event in command.Events)
        {
            result.Stage(@event);
        }

        foreach (var error in command.Result.Errors)
        {
            result.WithError(
                error.ErrorMessage!, 
                error.MemberNames.ToArray());
        }

        return result;
    }
}