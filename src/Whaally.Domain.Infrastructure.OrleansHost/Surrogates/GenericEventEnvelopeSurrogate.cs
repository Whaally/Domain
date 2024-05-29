using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Event;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct GenericEventEnvelopeSurrogate<TEvent>
    where TEvent : class, IEvent
{
    [Id(0)] public TEvent Message;
    [Id(1)] public IEventMetadata Metadata;
}