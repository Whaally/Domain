using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public record AnotherCommand() : ICommand;

public class AnotherCommandHandler : ICommandHandler<Aggregate, AnotherCommand>
{
    public IEnumerable<IReason> Evaluate(ICommandHandlerContext<Aggregate> context, AnotherCommand command)
    {
        context.StageEvent(new TestEvent());
        
        return [];
    }
}