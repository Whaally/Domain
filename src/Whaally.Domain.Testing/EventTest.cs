using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Testing;

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
            ParentContext = default,
            Aggregate = Aggregate
        };

        UpdatedAggregate = Handler.Apply(Context, Event);
    }
}
