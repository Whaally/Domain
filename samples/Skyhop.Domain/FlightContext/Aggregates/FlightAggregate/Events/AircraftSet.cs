using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events
{
    [Immutable]
    [GenerateSerializer]
    public record AircraftSet(string AircraftId) : IEvent;

    internal class AircraftSetHandler : IEventHandler<Flight, AircraftSet>
    {
        public Flight Apply(IEventHandlerContext<Flight> context, AircraftSet @event)
            => context.Aggregate with
            {
                AircraftId = @event.AircraftId
            };
    }
}
