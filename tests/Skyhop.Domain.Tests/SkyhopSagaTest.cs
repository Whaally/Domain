using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Saga;
using Whaally.Domain.Tests;

namespace Skyhop.Domain.Tests;

public abstract class SkyhopSagaTest<TEvent> : SagaTest<TEvent>
    where TEvent : class, IEvent
{
    protected SkyhopSagaTest(
        ISaga<TEvent> saga, 
        TEvent @event) : base(saga, @event) { }
    
    // ToDo: dobuble check if a saga implemented on Orleans requires any attributes
}
