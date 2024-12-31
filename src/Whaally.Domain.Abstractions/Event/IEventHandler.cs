namespace Whaally.Domain.Abstractions;

public interface IEventHandler : IMessageHandler
{
    public TAggregate Apply<TAggregate>(IEventHandlerContext<TAggregate> context, IEvent @event)
        where TAggregate : class, IAggregate;
}

public interface IEventHandler<TAggregate, TEvent> : IEventHandler
    where TAggregate : class, IAggregate
    where TEvent : class, IEvent
{
    T IEventHandler.Apply<T>(IEventHandlerContext<T> context, IEvent @event) =>
        (Apply(
            (IEventHandlerContext<TAggregate>)context, 
            (TEvent)@event) as T)!;

    public TAggregate Apply(IEventHandlerContext<TAggregate> context, TEvent @event);
}
