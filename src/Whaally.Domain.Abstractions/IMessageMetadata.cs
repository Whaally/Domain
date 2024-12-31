using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public interface IMessageMetadata
{
    public IDictionary<string, object> Attributes { get; set; }
    public ActivityContext? ParentContext { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    public string? TransactionId { get; set; }
}

public interface IServiceMetadata : IMessageMetadata
{
    public Type? ServiceType { get; set; }
}

public interface ICommandMetadata : IMessageMetadata
{
    public string AggregateId { get; set; }
    public Type? AggregateType { get; set; }
}

public interface IEventMetadata : IMessageMetadata
{
    public string AggregateId { get; set; }
    public Type? AggregateType { get; set; }
}
