using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public record CommandMetadata : IMessageMetadata
{
    public CommandMetadata() { }

    public CommandMetadata(IMessageMetadata metadata)
    {
        Attributes = metadata.Attributes;
        ParentContext = metadata.ParentContext;
        CreatedAt = metadata.CreatedAt;
        TransactionId = metadata.TransactionId;
    }

    public CommandMetadata(CommandMetadata metadata)
    {
        Attributes = metadata.Attributes;
        ParentContext = metadata.ParentContext;
        CreatedAt = metadata.CreatedAt;
        TransactionId = metadata.TransactionId;

        AggregateId = metadata.AggregateId;
        AggregateType = metadata.AggregateType;
    }
    
    public IDictionary<string, object> Attributes { get; set; } 
        = new Dictionary<string, object>();
    
    public ActivityContext? ParentContext { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }

    // There are valid reasons for why the AggregateId might not be set.
    // One of them is because the command is supplied to an AggregateHandler instance
    // thus already containing a reference to the aggregate.
    public Guid AggregateId { get; set; }
    
    public Type? AggregateType { get; set; }
    
    public string? TransactionId { get; set; }
}
