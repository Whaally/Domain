using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public record EventMetadata : IMessageMetadata
{
    public string AggregateId { get; set; } = "";

    public Type? AggregateType { get; set; }

    public IDictionary<string, object> Attributes { get; set; }
        = new Dictionary<string, object>();
    
    public ActivityContext? ParentContext { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public string? TransactionId { get; set; }
}
