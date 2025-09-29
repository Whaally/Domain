using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization;

[Immutable, GenerateSerializer]
public record TestCommand() : ICommand;

public class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
{
    public ICommandResult Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
    {
        return new Command().Stage(new TestEvent(true));
    }
}
