using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class EventHandlerContext<TAggregate> : IEventHandlerContext<TAggregate>
    where TAggregate : class, IAggregate
{
    public EventHandlerContext(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }
    
    public Guid AggregateId { get; init; }
    public string? TransactionId { get; init; }
    public TAggregate Aggregate { get; init; } = null!;
    public ActivityContext? ParentContext { get; init; }
    public IReadOnlyDictionary<string, object> Attributes { get; init; } 
        = new Dictionary<string, object>();
}
