using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public record TestCommand : ICommand;

public class TestCommandHandler : ICommandHandler<Aggregate, TestCommand>
{
    // todo: reconsider this test. This one is most likely to fail as the execution dynamics had changed
    public ICommandResult Evaluate(ICommandHandlerContext<Aggregate> context, TestCommand command)
    {
        var result = Command.Invoke(new AnotherCommand());

        if (context.Aggregate.EventApplicationCount != 1)
            result.WithError("Event application count is not 1");

        return result;
    }
}
