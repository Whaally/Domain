using Whaally.Domain.Abstractions;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class EventEnvelopeConverter : IConverter<EventEnvelope, EventEnvelopeSurrogate>
{
    public EventEnvelope ConvertFromSurrogate(in EventEnvelopeSurrogate surrogate) =>
        new(
            surrogate.Metadata,
            surrogate.Messages);

    public EventEnvelopeSurrogate ConvertToSurrogate(in EventEnvelope value) =>
        new()
        {
            Messages = value.Messages,
            Metadata = value.Metadata
        };
}
