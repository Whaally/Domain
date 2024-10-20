using Whaally.Domain.Abstractions.Event;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization;

[Immutable, GenerateSerializer]
public record TestEvent(bool Flag) : IEvent;

public class TestEventHandler : IEventHandler<TestAggregate, TestEvent>
{
    public TestAggregate Apply(IEventHandlerContext<TestAggregate> context, TestEvent @event)
        => context.Aggregate with
        {
            DidApplyEvent = @event.Flag
        };
}
