using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Event;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class EventEnvelopeConverter : IConverter<EventEnvelope, EventEnvelopeSurrogate>
{
    public EventEnvelope ConvertFromSurrogate(in EventEnvelopeSurrogate surrogate)
        => new EventEnvelope(
            surrogate.Message,
            surrogate.Metadata);

    public EventEnvelopeSurrogate ConvertToSurrogate(in EventEnvelope value)
        => new EventEnvelopeSurrogate
        {
            Message = value.Message,
            Metadata = value.Metadata
        };
}

[RegisterConverter]
public sealed class GenericEventEnvelopeConverter<TEvent> : IConverter<EventEnvelope<TEvent>, GenericEventEnvelopeSurrogate<TEvent>>
    where TEvent : class, IEvent
{
    public EventEnvelope<TEvent> ConvertFromSurrogate(in GenericEventEnvelopeSurrogate<TEvent> surrogate)
        => new EventEnvelope<TEvent>(
            surrogate.Message,
            surrogate.Metadata);

    public GenericEventEnvelopeSurrogate<TEvent> ConvertToSurrogate(in EventEnvelope<TEvent> value)
        => new GenericEventEnvelopeSurrogate<TEvent>
        {
            Message = value.Message,
            Metadata = value.Metadata
        };
}