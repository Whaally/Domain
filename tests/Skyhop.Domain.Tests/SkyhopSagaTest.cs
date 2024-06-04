using Whaally.Domain;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Saga;
using Whaally.Domain.Tests;

namespace Skyhop.Domain.Tests;

public abstract class SkyhopSagaTest<TEvent> : SagaTest<TEvent>
    where TEvent : class, IEvent
{
    public SkyhopSagaTest(
        Action<DomainContext> initializer,
        ISaga<TEvent> saga,
        IEventEnvelope<TEvent> @event) : base(initializer, saga, @event) { }
    
    public SkyhopSagaTest(
        Action<DomainContext> initializer,
        ISaga<TEvent> saga,
        TEvent @event) : base(initializer, saga, @event) { } 
    
    public SkyhopSagaTest(
        ISaga<TEvent> saga, 
        IEventEnvelope<TEvent> @event) : base(saga, @event) { }
    
    public SkyhopSagaTest(
        ISaga<TEvent> saga, 
        TEvent @event) : base(saga, @event) { }
    
    // ToDo: dobuble check if a saga implemented on Orleans requires any attributes
}
