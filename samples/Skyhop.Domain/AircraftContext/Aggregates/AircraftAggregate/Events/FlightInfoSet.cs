using Skyhop.Domain.Infrastructure;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;

[Immutable]
[GenerateSerializer]
public record FlightInfoSet(
    Guid FlightId,
    DateTime? DepartureTime,
    DateTime? ArrivalTime) : IEvent;

[GenerateMetadata]
public partial class FlightInfoSetHandler : IEventHandler<Aircraft, FlightInfoSet>
{
    public Aircraft Apply(IEventHandlerContext<Aircraft> context, FlightInfoSet @event)
    {
        var flights = context.Aggregate.Flights;

        // If any of these fields are null, use the previous value.
        flights.InsertOrUpdate(
            @event.FlightId,
            (o) => (
                @event.DepartureTime ?? o.Departure,
                @event.ArrivalTime ?? o.Arrival));

        return context.Aggregate with
        {
            Flights = flights
        };
    }
}