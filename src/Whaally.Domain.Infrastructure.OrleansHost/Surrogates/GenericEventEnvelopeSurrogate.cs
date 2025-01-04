using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct GenericEventEnvelopeSurrogate<TEvent>
    where TEvent : class, IEvent
{
    [Id(0)] public TEvent Message;
    [Id(1)] public EventMetadata Metadata;
}