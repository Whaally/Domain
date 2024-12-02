using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record ServiceEnvelope<TService>(
    TService Message,
    IServiceMetadata Metadata) : IServiceEnvelope<TService>
    where TService : class, IService
{
    public static implicit operator ServiceEnvelope<TService>(TService service) =>
        new(
            service,
            new ServiceMetadata
            {
                Timestamp = DateTime.UtcNow
            });
}