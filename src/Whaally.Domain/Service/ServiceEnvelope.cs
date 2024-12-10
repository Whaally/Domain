using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record ServiceEnvelope : IServiceEnvelope
{
    public ServiceEnvelope(
        IServiceMetadata metadata,
        IEnumerable<IService> messages)
    {
        Metadata = metadata;
        Messages = messages;
    }

    public ServiceEnvelope(
        IServiceMetadata metadata,
        params IService[] messages)
    {
        Metadata = metadata;
        Messages = messages;
    }
    
    public IServiceMetadata Metadata { get; init; }
    public IEnumerable<IService> Messages { get; init; }
}
