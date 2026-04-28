namespace Whaally.Domain.Abstractions;

public interface ICommandHandler : IMessageHandler
{
    public ICommandResult Evaluate(ICommandHandlerContext context, ICommand command);
}

public interface ICommandHandler<TAggregate, TCommand> : ICommandHandler
    where TAggregate : class, IAggregate
    where TCommand : class, ICommand
{
    ICommandResult ICommandHandler.Evaluate(ICommandHandlerContext context, ICommand command) => 
        Evaluate((ICommandHandlerContext<TAggregate>)context, (TCommand)command);

    public ICommandResult Evaluate(ICommandHandlerContext<TAggregate> context, TCommand command);
}
