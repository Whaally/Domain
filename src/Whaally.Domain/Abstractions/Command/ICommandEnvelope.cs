namespace Whaally.Domain.Abstractions;

public interface ICommandEnvelope : IMessageEnvelope
{
    public new IEnumerable<ICommand> Messages { get; }
    public new ICommandMetadata Metadata { get; }

    IEnumerable<IMessage> IMessageEnvelope.Messages => Messages;
    IMessageMetadata IMessageEnvelope.Metadata => Metadata;
}
