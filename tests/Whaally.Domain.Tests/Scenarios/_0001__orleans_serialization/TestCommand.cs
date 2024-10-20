using FluentResults;
using Whaally.Domain.Abstractions.Command;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization;

[Immutable, GenerateSerializer]
public record TestCommand() : ICommand;

public class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
{
    public IResultBase Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
    {
        var result = new Result();
        
        if (result.IsSuccess)
            context.StageEvent(new TestEvent(true));

        return result;
    }
}
