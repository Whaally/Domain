using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface ICommandHandler : IMessageHandler
{
    public IEnumerable<IReason> Evaluate(ICommandHandlerContext context, ICommand command);
}

public interface ICommandHandler<TAggregate, TCommand> : ICommandHandler
    where TAggregate : class, IAggregate
    where TCommand : class, ICommand
{
    IEnumerable<IReason> ICommandHandler.Evaluate(ICommandHandlerContext context, ICommand command) => 
        Evaluate((ICommandHandlerContext<TAggregate>)context, (TCommand)command);

    public IEnumerable<IReason> Evaluate(ICommandHandlerContext<TAggregate> context, TCommand command);
}
