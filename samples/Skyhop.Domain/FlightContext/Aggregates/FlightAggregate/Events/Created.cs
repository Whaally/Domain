using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record Created() : IEvent;

public class CreatedHandler : IEventHandler<Flight, Created>
{
    public Flight Apply(IEventHandlerContext<Flight> context, Created @event) =>
        context.Aggregate with
        {
            IsInitialized = true
        };
}