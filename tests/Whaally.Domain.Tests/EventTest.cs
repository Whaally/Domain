using FluentAssertions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Event;

namespace Whaally.Domain.Tests;

public abstract class EventTest<TAggregate, TEvent> : DomainTest
    where TAggregate : class, IAggregate, new()
    where TEvent : class, IEvent
{
    public IEventHandler<TAggregate, TEvent> Handler { get; } 
    public TAggregate Aggregate { get; }
    public TEvent Event { get; }
    
    public EventHandlerContext<TAggregate> Context { get; }
    public TAggregate UpdatedAggregate { get; }
    
    public EventTest(
        IEventHandler<TAggregate, TEvent> handler,
        TAggregate aggregate,
        TEvent @event)
    {
        Handler = handler;
        Aggregate = aggregate;
        Event = @event;
        
        var id = Guid.NewGuid();
        Context = new EventHandlerContext<TAggregate>(id.ToString())
        {
            Activity = default,
            Aggregate = Aggregate
        };

        UpdatedAggregate = Handler.Apply(Context, Event);
    }
}
