using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public record ServiceEnvelope(
    IServiceMetadata Metadata,
    IService Message) : IServiceEnvelope;
