using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record ServiceEnvelope(
    ServiceMetadata Metadata,
    IService Message) : IMessageEnvelope
{
    IEnumerable<IMessage> IMessageEnvelope.Messages => [ Message ];
    IMessageMetadata IMessageEnvelope.Metadata => Metadata;
}
