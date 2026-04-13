using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record AircraftRemoved(Guid AircraftId) : IEvent;

[GenerateMetadata]
public partial class AircraftRemovedHandler : IEventHandler<Flight, AircraftRemoved>
{
    public Flight Apply(IEventHandlerContext<Flight> context, AircraftRemoved @event) =>
        context.Aggregate with
        {
            AircraftId = Guid.Empty,
            AircraftRegistration = null
        };
}