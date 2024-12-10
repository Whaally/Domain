namespace Whaally.Domain.Abstractions;

public interface IEventEnvelope : IMessageEnvelope
{
    public new IEnumerable<IEvent> Messages { get; }
    public new IEventMetadata Metadata { get; }

    IEnumerable<IMessage> IMessageEnvelope.Messages
    {
        get => Messages;
    }

    IMessageMetadata IMessageEnvelope.Metadata
    {
        get => Metadata;
    }
}
