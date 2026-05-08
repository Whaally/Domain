using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization;

[Immutable, GenerateSerializer]
public record TestCommand() : ICommand;

public class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
{
    public async Task<ICommandResult> Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
    {
        return Command.Stage(new TestEvent(true));
    }
}
