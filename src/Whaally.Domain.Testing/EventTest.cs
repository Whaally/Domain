using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Testing;

public abstract class EventTest<TAggregate, TEvent> : DomainTest
    where TAggregate : class, IAggregate, new()
    where TEvent : class, IEvent
{
    public IEventHandler Handler { get; } 
    public TAggregate Aggregate { get; }
    public TEvent Event { get; }
    
    public EventHandlerContext<TAggregate> Context { get; }
    public TAggregate UpdatedAggregate { get; }
    
    public EventTest(
        TAggregate aggregate,
        TEvent @event)
    {
        Handler = Domain.GetEventHandler(typeof(TEvent));
        Aggregate = aggregate;
        Event = @event;
        
        var id = Guid.NewGuid();
        Context = new EventHandlerContext<TAggregate>(id)
        {
            ParentContext = default,
            Aggregate = Aggregate
        };

        UpdatedAggregate = Handler.Apply(Context, Event);
    }
}
