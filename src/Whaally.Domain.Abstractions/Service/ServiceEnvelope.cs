namespace Whaally.Domain.Abstractions;

public record ServiceEnvelope(
    ServiceMetadata Metadata,
    IService Message) : IMessageEnvelope
{
    IEnumerable<IMessage> IMessageEnvelope.Messages => [ Message ];
    IMessageMetadata IMessageEnvelope.Metadata => Metadata;
}
