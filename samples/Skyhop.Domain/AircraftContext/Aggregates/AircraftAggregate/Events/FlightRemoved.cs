using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events
{
    [Immutable]
    [GenerateSerializer]
    public record FlightRemoved(string FlightId) : IEvent;

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
