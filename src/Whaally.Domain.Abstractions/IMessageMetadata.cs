using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public interface IMessageMetadata
{
    public IDictionary<string, object> Attributes { get; set; }
    public ActivityContext? ParentContext { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    public string? TransactionId { get; set; }
}
