using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class ICommandMetadataConverter : IConverter<CommandMetadata, CommandMetadataSurrogate>
{
    public CommandMetadata ConvertFromSurrogate(in CommandMetadataSurrogate surrogate) =>
        new()
        {
            AggregateId = surrogate.AggregateId,
            AggregateType = surrogate.AggregateType,
            CreatedAt = surrogate.CreatedAt,
            Attributes = surrogate.Attributes,
            ParentContext = surrogate.SourceActivity
        };

    public CommandMetadataSurrogate ConvertToSurrogate(in CommandMetadata value) =>
        new()
        {
            AggregateId = value.AggregateId,
            AggregateType = value.AggregateType,
            CreatedAt = value.CreatedAt,
            Attributes = value.Attributes,
            SourceActivity = value.ParentContext
        };
}