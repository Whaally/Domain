using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization;

public record TestAggregate : IAggregate
{
    public bool DidApplyEvent { get; init; } = false;
}