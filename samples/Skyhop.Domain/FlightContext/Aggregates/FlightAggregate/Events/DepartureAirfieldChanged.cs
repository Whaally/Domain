using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record DepartureAirfieldChanged(
    string PreviousAirfieldId,
    string CurrentAirfieldId) : IEvent;

public class DepartureAirfieldChangedHandler : IEventHandler<Flight, DepartureAirfieldChanged>
{
    public Flight Apply(IEventHandlerContext<Flight> context, DepartureAirfieldChanged @event) =>
        context.Aggregate with
        {
            DepartureAirfieldId = @event.CurrentAirfieldId
        };
}