namespace Whaally.Domain.Abstractions;

public record CommandEnvelope : IMessageEnvelope
{
    public CommandEnvelope(
        CommandMetadata metadata,
        IEnumerable<ICommand> messages)
    {
        Metadata = metadata;
        Messages = messages;
    }

    public CommandEnvelope(
        CommandMetadata metadata,
        params ICommand[] messages)
    {
        Metadata = metadata;
        Messages = messages;
    }

    public CommandMetadata Metadata { get; init; }
    public IEnumerable<ICommand> Messages { get; init; }
    
    IEnumerable<IMessage> IMessageEnvelope.Messages => Messages;
    IMessageMetadata IMessageEnvelope.Metadata => Metadata;
}
