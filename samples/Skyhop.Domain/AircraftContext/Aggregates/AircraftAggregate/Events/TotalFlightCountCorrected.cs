using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;

[Immutable]
[GenerateSerializer]
public record TotalFlightCountCorrected(
    DateTime Timestamp,
    int Number) : IEvent;

[GenerateMetadata]
public partial class TotalFlightCountCorrectedHandler : IEventHandler<Aircraft, TotalFlightCountCorrected>
{
    public Aircraft Apply(IEventHandlerContext<Aircraft> context, TotalFlightCountCorrected @event)
        => context.Aggregate with
        {
            Starts = @event.Number
        };
}