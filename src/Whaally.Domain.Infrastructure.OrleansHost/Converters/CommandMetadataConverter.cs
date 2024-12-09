using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class ICommandMetadataConverter : IConverter<CommandMetadata, CommandMetadataSurrogate>
{
    public CommandMetadata ConvertFromSurrogate(in CommandMetadataSurrogate surrogate) =>
        new()
        {
            AggregateId = surrogate.AggregateId,
            CreatedAt = surrogate.CreatedAt,
            ParentContext = surrogate.SourceActivity
        };

    public CommandMetadataSurrogate ConvertToSurrogate(in CommandMetadata value) =>
        new()
        {
            AggregateId = value.AggregateId,
            CreatedAt = value.CreatedAt,
            SourceActivity = value.ParentContext
        };
}