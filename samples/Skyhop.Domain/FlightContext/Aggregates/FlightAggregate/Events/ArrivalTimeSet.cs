using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events
{
    public record ArrivalTimeSet(DateTime ArrivalTime) : IEvent;

    internal class ArrivalTimeSetHandler : IEventHandler<Flight, ArrivalTimeSet>
    {
        public Flight Apply(IEventHandlerContext<Flight> context, ArrivalTimeSet @event)
            => context.Aggregate with
            {
                ArrivalTime = @event.ArrivalTime
            };
    }
}
