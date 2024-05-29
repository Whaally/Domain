using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Abstractions.Event;

public interface IEventHandlerContext : IContext, IProvideAggregateInstance
{

}

public interface IEventHandlerContext<TAggregate> : IEventHandlerContext, IProvideAggregateInstance<TAggregate>
    where TAggregate : class, IAggregate
{

}