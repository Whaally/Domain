using Whaally.Domain.Abstractions.Event;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public record TestEvent : IEvent;

public class TestEventHandler : IEventHandler<Aggregate, TestEvent>
{
    public Aggregate Apply(IEventHandlerContext<Aggregate> context, TestEvent @event)
        => context.Aggregate with
        {
            EventApplicationCount = context.Aggregate.EventApplicationCount + 1
        };
}
