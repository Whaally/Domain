using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public interface IMessageMetadata
{
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Stores information about the activity from which this operation originated, including TraceId and SpanId.
    /// </summary>
    public ActivityContext SourceActivity { get; init; }

    // ToDo: consider adding authorization information as well
}

public interface IEventMetadata : IMessageMetadata
{
    public string AggregateId { get; init; }
}

public interface ICommandMetadata : IMessageMetadata
{
    public string AggregateId { get; init; }
}

public interface IServiceMetadata : IMessageMetadata
{

}