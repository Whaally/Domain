using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Tests.Domain;

internal record TestAggregate : IAggregate
{
    public Guid Id { get; init; }
}