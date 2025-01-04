using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record EventEnvelope : IMessageEnvelope
{
    public EventEnvelope(
        EventMetadata metadata,
        IEnumerable<IEvent> messages)
    {
        Messages = messages;
        Metadata = metadata;
    }

    public EventEnvelope(
        EventMetadata metadata,
        params IEvent[] messages)
    {
        Metadata = metadata;
        Messages = messages;
    }

    public EventMetadata Metadata { get; init; }
    public IEnumerable<IEvent> Messages { get; init; }
    
    IEnumerable<IMessage> IMessageEnvelope.Messages => Messages;
    IMessageMetadata IMessageEnvelope.Metadata => Metadata;
}
