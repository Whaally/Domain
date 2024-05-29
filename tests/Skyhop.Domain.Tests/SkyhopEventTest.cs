using FluentAssertions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Tests;

namespace Skyhop.Domain.Tests;

public abstract class SkyhopEventTest<TAggregate, TEvent> : EventTest<TAggregate, TEvent>
    where TAggregate : class, IAggregate, new()
    where TEvent : class, IEvent
{
    public SkyhopEventTest(
        IEventHandler<TAggregate, TEvent> handler, 
        TAggregate aggregate, 
        TEvent @event) : base(handler, aggregate, @event) { }
    
    [Fact]
    public void AssertCommandSerializerIsGenerated()
    {
        typeof(TEvent).Should().BeDecoratedWith<GenerateSerializerAttribute>();
    }

    [Fact]
    public void AssertCommandIsMarkedImmutable()
    {
        typeof(TEvent).Should().BeDecoratedWith<ImmutableAttribute>();
    }
}