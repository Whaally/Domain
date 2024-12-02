namespace Whaally.Domain.Abstractions;

public interface IEventHandlerContext : IContext, IProvideAggregateInstance
{

}

public interface IEventHandlerContext<TAggregate> : IEventHandlerContext, IProvideAggregateInstance<TAggregate>
    where TAggregate : class, IAggregate
{

}