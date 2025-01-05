using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization;

[Immutable, GenerateSerializer]
public record TestCommand() : ICommand;

public class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
{
    public IEnumerable<IReason> Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
    {
        context.StageEvent(new TestEvent(true));

        return [];
    }
}
