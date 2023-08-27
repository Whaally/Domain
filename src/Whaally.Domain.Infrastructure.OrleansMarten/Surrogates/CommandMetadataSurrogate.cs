using System.Diagnostics;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Surrogates
{
    [GenerateSerializer]
    public struct CommandMetadataSurrogate
    {
        [Id(0)] public string AggregateId;
        [Id(1)] public DateTime Timestamp;
        [Id(2)] public ActivityContext SourceActivity;
    }
}
