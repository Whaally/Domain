namespace Whaally.Domain.Abstractions;

public interface IServiceEnvelope : IMessageEnvelope
{
    public new IEnumerable<IService> Messages { get; }
    public new IServiceMetadata Metadata { get; }

    IEnumerable<IMessage> IMessageEnvelope.Messages
    {
        get => Messages;
    }

    IMessageMetadata IMessageEnvelope.Metadata
    {
        get => Metadata;
    }
}
