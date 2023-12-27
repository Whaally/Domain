namespace Whaally.Domain.Abstractions.Command;

public interface ICommandEnvelope : IMessageEnvelope
{
    public new ICommand Message { get; }
    public new ICommandMetadata Metadata { get; }

    IMessage IMessageEnvelope.Message
    {
        get => Message;
    }

    IMessageMetadata IMessageEnvelope.Metadata
    {
        get => Metadata;
    }
}

public interface ICommandEnvelope<out TCommand> : ICommandEnvelope
    where TCommand : ICommand
{
    public new TCommand Message { get; }

    ICommand ICommandEnvelope.Message
    {
        get => Message;
    }
}