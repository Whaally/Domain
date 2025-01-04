using Whaally.Domain.Abstractions;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class CommandEnvelopeConverter : IConverter<CommandEnvelope, CommandEnvelopeSurrogate>
{
    // TODO: Convert the metadata objects as well.
    // The commands work because they are decorated with the [GenerateSerializer] attribute as well.
    
    public CommandEnvelope ConvertFromSurrogate(in CommandEnvelopeSurrogate surrogate) =>
        new CommandEnvelope(
            surrogate.Metadata,
            surrogate.Messages);

    public CommandEnvelopeSurrogate ConvertToSurrogate(in CommandEnvelope value) =>
        new()
        {
            Metadata = value.Metadata,
            Messages = value.Messages
        };
}
