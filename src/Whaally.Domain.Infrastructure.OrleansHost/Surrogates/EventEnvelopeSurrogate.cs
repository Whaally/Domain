using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct EventEnvelopeSurrogate
{
    [Id(0)] public IEvent Message;
    [Id(1)] public IEventMetadata Metadata;
}