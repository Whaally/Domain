namespace Whaally.Domain.Abstractions.Service
{
    public interface IServiceEnvelope : IMessageEnvelope
    {
        public new IService Message { get; }
        public new IServiceMetadata Metadata { get; }

        IMessage IMessageEnvelope.Message
        {
            get => Message;
        }

        IMessageMetadata IMessageEnvelope.Metadata
        {
            get => Metadata;
        }
    }

    public interface IServiceEnvelope<out TService> : IServiceEnvelope
        where TService : IService
    {
        public new TService Message { get; }

        IService IServiceEnvelope.Message
        {
            get => Message;
        }
    }
}
