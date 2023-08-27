using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Event;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Surrogates
{
    [GenerateSerializer]
    public struct EventEnvelopeSurrogate
    {
        [Id(0)] public IEvent Message;
        [Id(1)] public IEventMetadata Metadata;
    }
}
