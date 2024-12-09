using System.Diagnostics;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct CommandMetadataSurrogate
{
    [Id(0)] public string AggregateId;
    [Id(1)] public DateTimeOffset CreatedAt;
    [Id(2)] public ActivityContext? SourceActivity;
}