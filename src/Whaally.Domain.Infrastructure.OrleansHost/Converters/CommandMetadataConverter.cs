using Whaally.Domain.Command;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class ICommandMetadataConverter : IConverter<CommandMetadata, CommandMetadataSurrogate>
{
    public CommandMetadata ConvertFromSurrogate(in CommandMetadataSurrogate surrogate) =>
        new()
        {
            AggregateId = surrogate.AggregateId,
            Timestamp = surrogate.Timestamp,
            SourceActivity = surrogate.SourceActivity
        };

    public CommandMetadataSurrogate ConvertToSurrogate(in CommandMetadata value) =>
        new()
        {
            AggregateId = value.AggregateId,
            Timestamp = value.Timestamp,
            SourceActivity = value.SourceActivity
        };
}