using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public record EventMetadata : IMessageMetadata
{
    public EventMetadata() { }

    public EventMetadata(CommandMetadata command)
    {
        AggregateId = command.AggregateId;
        AggregateType = command.AggregateType;
        Attributes = command.Attributes;
        ParentContext = command.ParentContext;
        CreatedAt = command.CreatedAt;
        TransactionId = command.TransactionId;
    }
    
    public Guid AggregateId { get; set; }

    public Type? AggregateType { get; set; }

    public IDictionary<string, object> Attributes { get; set; }
        = new Dictionary<string, object>();
    
    public ActivityContext? ParentContext { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public string? TransactionId { get; set; }
}
