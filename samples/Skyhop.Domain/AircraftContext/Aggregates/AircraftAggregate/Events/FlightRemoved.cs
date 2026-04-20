using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;

[Immutable]
[GenerateSerializer]
public record FlightRemoved(Guid FlightId) : IEvent;

[GenerateMetadata]
public partial class FlightRemovedHandler : IEventHandler<Aircraft, FlightRemoved>
{
    public Aircraft Apply(IEventHandlerContext<Aircraft> context, FlightRemoved @event)
    {
        var flights = context.Aggregate.Flights;

        flights.Remove(@event.FlightId);

        return context.Aggregate with
        {
            Flights = flights
        };
    }
}