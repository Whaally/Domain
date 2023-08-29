using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events
{
    public record DepartureAirfieldChanged(
        string AggregateId,
        string PreviousAirfieldId,
        string CurrentAirfieldId) : IEvent;

    internal class DepartureAirfieldChangedHandler : IEventHandler<Flight, DepartureAirfieldChanged>
    {
        public Flight Apply(IEventHandlerContext<Flight> context, DepartureAirfieldChanged @event)
            => context.Aggregate with
            {
                DepartureAirfieldId = @event.CurrentAirfieldId
            };
    }
}
