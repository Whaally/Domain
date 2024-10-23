using FluentResults;
using Whaally.Domain.Abstractions.Command;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public record TestCommand : ICommand
{
    
}

public class TestCommandHandler : ICommandHandler<Aggregate, TestCommand>
{
    public IResultBase Evaluate(ICommandHandlerContext<Aggregate> context, TestCommand command)
    {
        context.EvaluateCommand(new AnotherCommand());

        return context.Aggregate.EventApplicationCount != 1 
            ? Result.Fail("Event application count is not 1") 
            : Result.Ok();
    }
}
