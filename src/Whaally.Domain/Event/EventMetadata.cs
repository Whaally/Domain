using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record EventMetadata : IEventMetadata
{
    public string AggregateId { get; set; } = "";

    public IDictionary<string, object> Attributes { get; set; }
        = new Dictionary<string, object>();
    
    public ActivityContext? ParentContext { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}
