using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public record Aggregate : IAggregate
{
    public int EventApplicationCount { get; init; }
}
