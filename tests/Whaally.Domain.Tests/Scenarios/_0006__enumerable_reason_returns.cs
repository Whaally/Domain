using FluentAssertions;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios;

public class _0006__enumerable_reason_returns
{
    class TestAggregate : IAggregate;

    class TestCommand : ICommand;
    class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
    {
        public async Task<ICommandResult> Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
        {
            return Command.WithError("Failure");
        }
    }

    // We need an actual command handler context instance with DI here :(
    // [Fact]
    // public void TestCommandHandlerEvaluationFails()
    // {
    //     var context = new CommandHandlerContext<TestAggregate>(null!, "");
    //     
    //     new TestCommandHandler()
    //         .Evaluate(context, new TestCommand());
    //         
    //     context.Result.Reasons.Should().ContainSingle();
    // }
}