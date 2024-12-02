using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class EventHandlerContext<TAggregate> : IEventHandlerContext<TAggregate>
    where TAggregate : class, IAggregate
{
    public EventHandlerContext(string aggregateId)
    {
        AggregateId = aggregateId;
    }

    // Warning; we're assuming aggregate is not null for end user convenience. Make this more explicit in a new version.
    public TAggregate Aggregate { get; init; } = null!;
    public ActivityContext Activity { get; init; }
    public string AggregateId { get; init; }
}
