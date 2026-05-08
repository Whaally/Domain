namespace Whaally.Domain.Abstractions;

public interface ICommandHandler : IMessageHandler
{
    public Task<ICommandResult> Evaluate(ICommandHandlerContext context, ICommand command);
}

public interface ICommandHandler<TAggregate, TCommand> : ICommandHandler
    where TAggregate : class, IAggregate
    where TCommand : class, ICommand
{
    Task<ICommandResult> ICommandHandler.Evaluate(ICommandHandlerContext context, ICommand command) => 
        Evaluate((ICommandHandlerContext<TAggregate>)context, (TCommand)command);

    public Task<ICommandResult> Evaluate(ICommandHandlerContext<TAggregate> context, TCommand command);
}
