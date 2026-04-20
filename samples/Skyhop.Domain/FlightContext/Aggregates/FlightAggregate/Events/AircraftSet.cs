using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record AircraftSet(Guid AircraftId) : IEvent;

[GenerateMetadata]
public partial class AircraftSetHandler : IEventHandler<Flight, AircraftSet>
{
    public Flight Apply(IEventHandlerContext<Flight> context, AircraftSet @event) =>
        context.Aggregate with
        {
            AircraftId = @event.AircraftId
        };
}