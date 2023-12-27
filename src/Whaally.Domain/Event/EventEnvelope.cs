using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Event;

namespace Whaally.Domain.Event;

public record EventEnvelope(
    IEvent Message,
    IEventMetadata Metadata) : IEventEnvelope;

public record EventEnvelope<TEvent>(
    TEvent Message,
    IEventMetadata Metadata) : IEventEnvelope<TEvent>
    where TEvent : class, IEvent;