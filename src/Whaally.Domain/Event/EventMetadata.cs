using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record EventMetadata(string AggregateId) : IEventMetadata
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public ActivityContext SourceActivity { get; init; }
}