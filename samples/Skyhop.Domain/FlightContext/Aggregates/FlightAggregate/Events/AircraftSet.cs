using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events
{
    public record AircraftSet(
        string AggregateId,
        string AircraftId) : IEvent;

    internal class AircraftSetHandler : IEventHandler<Flight, AircraftSet>
    {
        public Flight Apply(IEventHandlerContext<Flight> context, AircraftSet @event)
            => context.Aggregate with
            {
                AircraftId = @event.AircraftId
            };
    }
}
