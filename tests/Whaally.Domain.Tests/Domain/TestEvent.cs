using Whaally.Domain.Abstractions.Event;

namespace Whaally.Domain.Tests.Domain
{
    internal record TestEvent : IEvent
    {
        public Guid AggregateId { get; init; }
    }

    internal class TestEventHandler : IEventHandler<TestAggregate, TestEvent>
    {
        public TestAggregate Apply(IEventHandlerContext<TestAggregate> context, TestEvent @event)
            => context.Aggregate;
    }
}
