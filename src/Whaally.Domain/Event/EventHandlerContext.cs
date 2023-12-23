using System.Diagnostics;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Event;

namespace Whaally.Domain.Event;

public class EventHandlerContext<TAggregate> : IEventHandlerContext<TAggregate>
    where TAggregate : class, IAggregate, new()
{
    public EventHandlerContext(string aggregateId)
    {
            AggregateId = aggregateId;
        }

    public TAggregate Aggregate { get; init; } = new();
    public ActivityContext Activity { get; init; }
    public string AggregateId { get; init; }
}