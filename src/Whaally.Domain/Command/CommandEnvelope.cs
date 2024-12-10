using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record CommandEnvelope : ICommandEnvelope
{
    public CommandEnvelope(
        ICommandMetadata metadata,
        IEnumerable<ICommand> messages)
    {
        this.Metadata = metadata;
        this.Messages = messages;
    }

    public CommandEnvelope(
        ICommandMetadata metadata,
        params ICommand[] messages)
    {
        Metadata = metadata;
        Messages = messages;
    }

    public ICommandMetadata Metadata { get; init; }
    public IEnumerable<ICommand> Messages { get; init; }
}
