using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct EventEnvelopeSurrogate
{
    [Id(0)] public IEventMetadata Metadata;
    [Id(1)] public IEnumerable<IEvent> Messages;
}