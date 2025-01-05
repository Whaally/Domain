using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface ICommandHandler : IMessageHandler
{
    public void Evaluate(ICommandHandlerContext context, ICommand command);
}

public interface ICommandHandler<TAggregate, TCommand> : ICommandHandler
    where TAggregate : class, IAggregate
    where TCommand : class, ICommand
{
    void ICommandHandler.Evaluate(ICommandHandlerContext context, ICommand command) => 
        Evaluate((ICommandHandlerContext<TAggregate>)context, (TCommand)command);

    public void Evaluate(ICommandHandlerContext<TAggregate> context, TCommand command);
}
