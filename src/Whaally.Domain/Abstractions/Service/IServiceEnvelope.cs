namespace Whaally.Domain.Abstractions;

public interface IServiceEnvelope : IMessageEnvelope
{
    public IService Message { get; }
    public new IServiceMetadata Metadata { get; }

    IEnumerable<IMessage> IMessageEnvelope.Messages => [ Message ];
    IMessageMetadata IMessageEnvelope.Metadata => Metadata;
}
