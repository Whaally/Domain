using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Abstractions.Event;

public interface IEventHandler : IMessageHandler
{
    public TAggregate Apply<TAggregate>(IEventHandlerContext<TAggregate> context, IEvent @event)
        where TAggregate : class, IAggregate;
}

public interface IEventHandler<TAggregate, TEvent> : IEventHandler
    where TAggregate : class, IAggregate
    where TEvent : class, IEvent
{
    T IEventHandler.Apply<T>(IEventHandlerContext<T> context, IEvent @event)
    {
            // ToDo: Deal with the situation where @event.Message is IEvent and cannot be casted to TEvent.
            var _context = context as IEventHandlerContext<TAggregate>;

            return (Apply(_context!, (TEvent)@event) as T)!;
        }

    public TAggregate Apply(IEventHandlerContext<TAggregate> context, TEvent @event);
}