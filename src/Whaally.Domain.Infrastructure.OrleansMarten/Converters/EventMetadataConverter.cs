﻿using Whaally.Domain.Event;
using Whaally.Domain.Infrastructure.OrleansMarten.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Converters
{

    [RegisterConverter]
    public sealed class IEventMetadataConverter : IConverter<EventMetadata, EventMetadataSurrogate>
    {
        public EventMetadata ConvertFromSurrogate(in EventMetadataSurrogate surrogate)
            => new EventMetadata(surrogate.AggregateId)
            {
                Timestamp = surrogate.Timestamp,
                SourceActivity = surrogate.SourceActivity
            };

        public EventMetadataSurrogate ConvertToSurrogate(in EventMetadata value)
            => new EventMetadataSurrogate()
            {
                AggregateId = value.AggregateId,
                Timestamp = value.Timestamp,
                SourceActivity = value.SourceActivity
            };
    }
}
