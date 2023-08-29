using Skyhop.Domain.Infrastructure;
using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events
{
    public record FlightInfoSet(
        string AggregateId,
        string FlightId,
        DateTime? DepartureTime,
        DateTime? ArrivalTime) : IEvent;

    internal class FlightInfoSetHandler : IEventHandler<Aircraft, FlightInfoSet>
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
