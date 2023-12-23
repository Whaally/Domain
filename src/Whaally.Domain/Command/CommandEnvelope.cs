using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Command;

namespace Whaally.Domain.Command;

public record CommandEnvelope(
    ICommand Message,
    ICommandMetadata Metadata) : ICommandEnvelope;

public record CommandEnvelope<TCommand>(
    TCommand Message,
    ICommandMetadata Metadata) : ICommandEnvelope<TCommand>
    where TCommand : class, ICommand
{
    public static implicit operator CommandEnvelope<TCommand>(TCommand command)
        => new(
            command,
            new CommandMetadata
            {
                Timestamp = DateTime.UtcNow
            });
}