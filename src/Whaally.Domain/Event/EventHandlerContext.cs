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
    public IDictionary<string, object> Attributes { get; init; } = new Dictionary<string, object>();
    public ActivityContext? ParentContext { get; init; }
    public string AggregateId { get; init; }
}
