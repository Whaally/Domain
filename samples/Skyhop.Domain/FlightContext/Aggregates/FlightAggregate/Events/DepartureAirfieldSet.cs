using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record DepartureAirfieldSet(string AirfieldId) : IEvent;

public class DepartureAirfieldSetHandler : IEventHandler<Flight, DepartureAirfieldSet>
{
    public Flight Apply(IEventHandlerContext<Flight> context, DepartureAirfieldSet @event)
        => context.Aggregate with
        {
            DepartureAirfieldId = @event.AirfieldId
        };
}