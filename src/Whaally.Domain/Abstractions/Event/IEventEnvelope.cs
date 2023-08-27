namespace Whaally.Domain.Abstractions.Event
{
    public interface IEventEnvelope : IMessageEnvelope
    {
        public new IEvent Message { get; }
        public new IEventMetadata Metadata { get; }

        IMessage IMessageEnvelope.Message
        {
            get => Message;
        }

        IMessageMetadata IMessageEnvelope.Metadata
        {
            get => Metadata;
        }
    }

    public interface IEventEnvelope<out TEvent> : IEventEnvelope
        where TEvent : class, IEvent
    {
        public new TEvent Message { get; }

        IEvent IEventEnvelope.Message
        {
            get => Message;
        }
    }
}
