namespace Whaally.Domain.Abstractions;

public interface ICommandHandlerContext : IContext, IProvideAggregateInstance;

public interface ICommandHandlerContext<TAggregate>
    : ICommandHandlerContext, IProvideAggregateInstance<TAggregate>
    where TAggregate : class, IAggregate;
