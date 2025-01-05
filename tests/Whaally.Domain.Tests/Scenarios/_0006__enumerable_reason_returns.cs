using FluentAssertions;
using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios;

public class _0006__enumerable_reason_returns
{
    class TestAggregate : IAggregate;

    class TestCommand : ICommand;
    class TestCommandHandler : ICommandHandler<TestAggregate, TestCommand>
    {
        public IEnumerable<IReason> Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command)
        {
            yield return new Error("Failure");
        }
    }

    [Fact]
    public void TestCommandHandlerEvaluationFails()
    {
        new TestCommandHandler()
            .Evaluate(null!, new TestCommand())
            .Should().ContainSingle();
    }
}