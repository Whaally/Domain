using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Domain;

internal record TestAggregate : IAggregate
{
    public Guid Id { get; init; }
}