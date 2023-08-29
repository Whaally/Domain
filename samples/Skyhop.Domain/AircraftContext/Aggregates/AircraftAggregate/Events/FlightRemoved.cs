using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events
{
    public record FlightRemoved(
        string AggregateId,
        string FlightId) : IEvent;

    internal class FlightRemovedHandler : IEventHandler<Aircraft, FlightRemoved>
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
}
