using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public record ServiceMetadata : IMessageMetadata
{
    public ServiceMetadata() { }
    
    public ServiceMetadata(ServiceMetadata metadata)
    {
        Attributes = metadata.Attributes;
        ParentContext = metadata.ParentContext;
        CreatedAt = metadata.CreatedAt;
        TransactionId = metadata.TransactionId;
    }
    
    public IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
    public ActivityContext? ParentContext { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Type? ServiceType { get; set; }
    public string? TransactionId { get; set; }
}
