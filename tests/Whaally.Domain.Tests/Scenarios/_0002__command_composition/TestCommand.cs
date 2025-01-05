using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public record TestCommand : ICommand
{
    
}

public class TestCommandHandler : ICommandHandler<Aggregate, TestCommand>
{
    public IEnumerable<IReason> Evaluate(ICommandHandlerContext<Aggregate> context, TestCommand command)
    {
        context.EvaluateCommand(new AnotherCommand());

        return context.Aggregate.EventApplicationCount != 1 
            ? [ new Error("Event application count is not 1") ] 
            : [];
    }
}
