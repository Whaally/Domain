using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class IEventMetadataConverter : IConverter<EventMetadata, EventMetadataSurrogate>
{
    public EventMetadata ConvertFromSurrogate(in EventMetadataSurrogate surrogate) =>
        new()
        {
            AggregateId = surrogate.AggregateId,
            CreatedAt = surrogate.CreatedAt,
            ParentContext = surrogate.SourceActivity
        };

    public EventMetadataSurrogate ConvertToSurrogate(in EventMetadata value) =>
        new()
        {
            AggregateId = value.AggregateId,
            CreatedAt = value.CreatedAt,
            SourceActivity = value.ParentContext
        };
}