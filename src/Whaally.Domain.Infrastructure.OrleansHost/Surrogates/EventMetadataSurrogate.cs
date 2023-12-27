using System.Diagnostics;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct EventMetadataSurrogate
{
    [Id(0)] public string AggregateId;
    [Id(1)] public DateTime Timestamp;
    [Id(2)] public ActivityContext SourceActivity;
}