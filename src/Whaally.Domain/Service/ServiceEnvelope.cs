using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Service;

namespace Whaally.Domain.Service;

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