using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record EventEnvelope(
    IEvent Message,
    IEventMetadata Metadata) : IEventEnvelope;

public record EventEnvelope<TEvent>(
    TEvent Message,
    IEventMetadata Metadata) : IEventEnvelope<TEvent>
    where TEvent : class, IEvent;