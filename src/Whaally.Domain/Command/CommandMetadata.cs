using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record CommandMetadata : ICommandMetadata
{
    public IDictionary<string, object> Attributes { get; set; } 
        = new Dictionary<string, object>();
    
    public ActivityContext? ParentContext { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    // There are valid reasons for why the AggregateId might not be set.
    // One of them is because the command is supplied to an AggregateHandler instance
    // thus already containing a reference to the aggregate.
    public string AggregateId { get; set; }
        = "";
}
