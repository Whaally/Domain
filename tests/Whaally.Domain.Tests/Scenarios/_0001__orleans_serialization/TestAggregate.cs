using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization;

public record TestAggregate : IAggregate
{
    public bool DidApplyEvent { get; init; } = false;
}