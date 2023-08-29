using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events
{
    public record TotalFlightTimeCorrected(
        Guid AggregateId,
        DateTime Timestamp,
        TimeSpan FlightTime) : IEvent;

    public class TotalFlightTimeCorrectedHandler : IEventHandler<Aircraft, TotalFlightTimeCorrected>
    {
        public Aircraft Apply(IEventHandlerContext<Aircraft> context, TotalFlightTimeCorrected @event)
            => context.Aggregate with
            {
                FlightTime = @event.FlightTime
            };
    }
}
