using Whaally.Domain.Abstractions;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class CommandEnvelopeConverter : IConverter<CommandEnvelope, CommandEnvelopeSurrogate>
{
    public CommandEnvelope ConvertFromSurrogate(in CommandEnvelopeSurrogate surrogate) =>
        new(
            surrogate.Metadata,
            surrogate.Messages);

    public CommandEnvelopeSurrogate ConvertToSurrogate(in CommandEnvelope value) =>
        new()
        {
            Metadata = value.Metadata,
            Messages = value.Messages
        };
}
