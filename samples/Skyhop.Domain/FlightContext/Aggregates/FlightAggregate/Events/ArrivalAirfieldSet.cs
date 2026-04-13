using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record ArrivalAirfieldSet(Guid AirfieldId) : IEvent;

[GenerateMetadata]
public partial class ArrivalAirfieldSetHandler : IEventHandler<Flight, ArrivalAirfieldSet>
{
    public Flight Apply(IEventHandlerContext<Flight> context, ArrivalAirfieldSet @event) =>
        context.Aggregate with
        {
            ArrivalAirfieldId = @event.AirfieldId
        };
}