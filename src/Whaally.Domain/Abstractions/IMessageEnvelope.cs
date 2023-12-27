using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public interface IMessageEnvelope
{
    public IMessage Message { get; }
    public IMessageMetadata Metadata { get; }
}

public interface IMessageEnvelope<TMessage> : IMessageEnvelope
    where TMessage : class, IMessage
{
    public new TMessage Message { get; }

    IMessage IMessageEnvelope.Message
    {
        get => Message;
    }
}

public class MessageEnvelope<TMessage> : IMessageEnvelope<TMessage>
    where TMessage : class, IMessage
{
    public MessageEnvelope(
        TMessage message,
        IMessageMetadata metadata)
    {
        Message = message;
        Metadata = metadata;
    }

    public TMessage Message { get; }
    public IMessageMetadata Metadata { get; }
    public ActivityContext OriginActivity { get; }
}