using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record EventEnvelope : IEventEnvelope
{
    public EventEnvelope(
        IEventMetadata metadata,
        IEnumerable<IEvent> messages)
    {
        Messages = messages;
        Metadata = metadata;
    }

    public EventEnvelope(
        IEventMetadata metadata,
        params IEvent[] messages)
    {
        Metadata = metadata;
        Messages = messages;
    }

    public IEventMetadata Metadata { get; init; }
    public IEnumerable<IEvent> Messages { get; init; }
}
