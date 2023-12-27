using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Command;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class CommandEnvelopeConverter : IConverter<CommandEnvelope, CommandEnvelopeSurrogate>
{
    public CommandEnvelope ConvertFromSurrogate(in CommandEnvelopeSurrogate surrogate) =>
        new(
            surrogate.Message,
            surrogate.Metadata);

    public CommandEnvelopeSurrogate ConvertToSurrogate(in CommandEnvelope value) =>
        new()
        {
            Message = value.Message,
            Metadata = value.Metadata
        };
}

[RegisterConverter]
public sealed class ICommandEnvelopeConverter<TCommand> : IConverter<CommandEnvelope<TCommand>, CommandEnvelopeSurrogate<TCommand>>
    where TCommand : class, ICommand
{
    public CommandEnvelope<TCommand> ConvertFromSurrogate(in CommandEnvelopeSurrogate<TCommand> surrogate) =>
        new(
            surrogate.Message,
            surrogate.Metadata);

    public CommandEnvelopeSurrogate<TCommand> ConvertToSurrogate(in CommandEnvelope<TCommand> value) =>
        new()
        {
            Message = value.Message,
            Metadata = value.Metadata
        };
}
