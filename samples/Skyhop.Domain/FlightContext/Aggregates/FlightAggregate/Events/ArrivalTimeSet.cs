using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record ArrivalTimeSet(DateTime ArrivalTime) : IEvent;

public class ArrivalTimeSetHandler : IEventHandler<Flight, ArrivalTimeSet>
{
    public Flight Apply(IEventHandlerContext<Flight> context, ArrivalTimeSet @event) =>
        context.Aggregate with
        {
            ArrivalTime = @event.ArrivalTime
        };
}