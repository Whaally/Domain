using FluentResults;
using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Abstractions.Command;

public interface ICommandHandler : IMessageHandler
{
    public IResultBase Evaluate(ICommandHandlerContext context, ICommand command);
}

public interface ICommandHandler<TAggregate, TCommand> : ICommandHandler
    where TAggregate : class, IAggregate
    where TCommand : class, ICommand
{
    IResultBase ICommandHandler.Evaluate(ICommandHandlerContext context, ICommand command) => 
        Evaluate((ICommandHandlerContext<TAggregate>)context, (TCommand)command);

    public IResultBase Evaluate(ICommandHandlerContext<TAggregate> context, TCommand command);
}