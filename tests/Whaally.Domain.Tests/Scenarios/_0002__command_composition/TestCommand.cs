using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public record TestCommand : ICommand;

public class TestCommandHandler : ICommandHandler<Aggregate, TestCommand>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Aggregate> context, TestCommand command)
    {
        context.EvaluateCommand(new AnotherCommand());

        if (context.Aggregate.EventApplicationCount != 1)
            context.WithResult(Result.Fail("Event application count is not 1"));
    }
}
