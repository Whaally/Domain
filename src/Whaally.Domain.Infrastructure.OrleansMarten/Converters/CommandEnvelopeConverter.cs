using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Command;
using Whaally.Domain.Infrastructure.OrleansMarten.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Converters
{

    [RegisterConverter]
    public sealed class ICommandEnvelopeConverter<TCommand> : IConverter<CommandEnvelope<TCommand>, CommandEnvelopeSurrogate<TCommand>>
        where TCommand : class, ICommand
    {
        public CommandEnvelope<TCommand> ConvertFromSurrogate(in CommandEnvelopeSurrogate<TCommand> surrogate)
            => new CommandEnvelope<TCommand>(
                surrogate.Message,
                surrogate.Metadata);

        public CommandEnvelopeSurrogate<TCommand> ConvertToSurrogate(in CommandEnvelope<TCommand> value)
            => new CommandEnvelopeSurrogate<TCommand>
            {
                Message = value.Message,
                Metadata = value.Metadata
            };
    }
}
