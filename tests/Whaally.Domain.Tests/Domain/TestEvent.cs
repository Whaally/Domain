using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Domain;

[Immutable, GenerateSerializer]
internal record TestEvent : IEvent
{
    [Id(0)]
    public Guid AggregateId { get; init; }
}

internal class TestEventHandler : IEventHandler<TestAggregate, TestEvent>
{
    public TestAggregate Apply(IEventHandlerContext<TestAggregate> context, TestEvent @event)
        => context.Aggregate;
}