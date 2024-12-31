namespace Whaally.Domain.Abstractions;

public interface IMessageEnvelope
{
    public IEnumerable<IMessage> Messages { get; }
    public IMessageMetadata Metadata { get; }
}
