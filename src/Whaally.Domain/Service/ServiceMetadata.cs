using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record ServiceMetadata : IServiceMetadata
{
    public IDictionary<string, object> Attributes { get; set; }
        = new Dictionary<string, object>();
    public ActivityContext? ParentContext { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Type? ServiceType { get; set; }
    public string? TransactionId { get; set; }
}
