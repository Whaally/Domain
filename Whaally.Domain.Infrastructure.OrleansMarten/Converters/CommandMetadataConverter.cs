using Whaally.Domain.Command;
using Whaally.Domain.Infrastructure.OrleansMarten.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Converters
{

    [RegisterConverter]
    public sealed class ICommandMetadataConverter : IConverter<CommandMetadata, CommandMetadataSurrogate>
    {
        public CommandMetadata ConvertFromSurrogate(in CommandMetadataSurrogate surrogate)
            => new CommandMetadata
            {
                AggregateId = surrogate.AggregateId,
                Timestamp = surrogate.Timestamp,
                SourceActivity = surrogate.SourceActivity
            };

        public CommandMetadataSurrogate ConvertToSurrogate(in CommandMetadata value)
            => new CommandMetadataSurrogate
            {
                AggregateId = value.AggregateId,
                Timestamp = value.Timestamp,
                SourceActivity = value.SourceActivity
            };
    }
}
