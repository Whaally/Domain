using System.Diagnostics;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct CommandMetadataSurrogate
{
    [Id(0)] public string AggregateId;
    [Id(1)] public Type? AggregateType;
    [Id(2)] public DateTimeOffset CreatedAt;
    [Id(3)] public IDictionary<string, object> Attributes;
    [Id(4)] public ActivityContext? SourceActivity;
}
