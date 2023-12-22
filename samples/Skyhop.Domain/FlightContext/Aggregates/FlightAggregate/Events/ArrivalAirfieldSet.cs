using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events
{
    [Immutable]
    [GenerateSerializer]
    public record ArrivalAirfieldSet(string AirfieldId) : IEvent;

    internal class ArrivalAirfieldSetHandler : IEventHandler<Flight, ArrivalAirfieldSet>
    {
        public Flight Apply(IEventHandlerContext<Flight> context, ArrivalAirfieldSet @event)
            => context.Aggregate with
            {
                ArrivalAirfieldId = @event.AirfieldId
            };
    }
}
