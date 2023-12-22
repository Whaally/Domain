using Skyhop.Domain.Infrastructure;
using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events
{
    [Immutable]
    [GenerateSerializer]
    public record FlightInfoSet(
        string FlightId,
        DateTime? DepartureTime,
        DateTime? ArrivalTime) : IEvent;

    public class FlightInfoSetHandler : IEventHandler<Aircraft, FlightInfoSet>
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
}
