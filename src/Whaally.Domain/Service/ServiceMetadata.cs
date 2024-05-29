using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Service;

public record ServiceMetadata : IServiceMetadata
{
    public DateTime Timestamp { get; init; }
    public ActivityContext SourceActivity { get; init; }
}