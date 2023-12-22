using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events
{
    [Immutable]
    [GenerateSerializer]
    public record DepartureTimeSet(DateTime DepartureTime) : IEvent;

    public class DepartureTimeSetHandler : IEventHandler<Flight, DepartureTimeSet>
    {
        public Flight Apply(IEventHandlerContext<Flight> context, DepartureTimeSet @event)
            => context.Aggregate with
            {
                DepartureTime = @event.DepartureTime
            };
    }
}
