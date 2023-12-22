using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events
{
    [Immutable]
    [GenerateSerializer]
    public record TotalFlightCountCorrected(
        DateTime Timestamp,
        int Number) : IEvent;

    public class TotalFlightCountCorrectedHandler : IEventHandler<Aircraft, TotalFlightCountCorrected>
    {
        public Aircraft Apply(IEventHandlerContext<Aircraft> context, TotalFlightCountCorrected @event)
            => context.Aggregate with
            {
                Starts = @event.Number
            };
    }
}
