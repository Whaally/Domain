using Whaally.Domain.Abstractions;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class CommandMetadataConverter : IConverter<ICommandMetadata, CommandMetadataSurrogate>
{
    public ICommandMetadata ConvertFromSurrogate(in CommandMetadataSurrogate surrogate) =>
        new CommandMetadata()
        {
            AggregateId = surrogate.AggregateId,
            AggregateType = surrogate.AggregateType,
            CreatedAt = surrogate.CreatedAt,
            Attributes = surrogate.Attributes,
            ParentContext = surrogate.SourceActivity,
            TransactionId = surrogate.TransactionId
        };

    public CommandMetadataSurrogate ConvertToSurrogate(in ICommandMetadata value) =>
        new()
        {
            AggregateId = value.AggregateId,
            AggregateType = value.AggregateType,
            CreatedAt = value.CreatedAt,
            Attributes = value.Attributes,
            SourceActivity = value.ParentContext,
            TransactionId  = value.TransactionId
        };
}
