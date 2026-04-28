using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public record AnotherCommand() : ICommand;

public class AnotherCommandHandler : ICommandHandler<Aggregate, AnotherCommand>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Aggregate> context, AnotherCommand command)
    {
        return Command.Stage(new TestEvent());
    }
}
