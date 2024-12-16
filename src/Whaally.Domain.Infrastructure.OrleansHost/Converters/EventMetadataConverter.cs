using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class IEventMetadataConverter : IConverter<EventMetadata, EventMetadataSurrogate>
{
    public EventMetadata ConvertFromSurrogate(in EventMetadataSurrogate surrogate) =>
        new()
        {
            AggregateId = surrogate.AggregateId,
            AggregateType = surrogate.AggregateType,
            CreatedAt = surrogate.CreatedAt,
            Attributes = surrogate.Attributes,
            ParentContext = surrogate.SourceActivity,
            TransactionId = surrogate.TransactionId
        };

    public EventMetadataSurrogate ConvertToSurrogate(in EventMetadata value) =>
        new()
        {
            AggregateId = value.AggregateId,
            AggregateType = value.AggregateType,
            CreatedAt = value.CreatedAt,
            Attributes = value.Attributes,
            SourceActivity = value.ParentContext,
            TransactionId = value.TransactionId
        };
}
