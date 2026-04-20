using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record DepartureAirfieldChanged(
    Guid PreviousAirfieldId,
    Guid CurrentAirfieldId) : IEvent;

[GenerateMetadata]
public partial class DepartureAirfieldChangedHandler : IEventHandler<Flight, DepartureAirfieldChanged>
{
    public Flight Apply(IEventHandlerContext<Flight> context, DepartureAirfieldChanged @event) =>
        context.Aggregate with
        {
            DepartureAirfieldId = @event.CurrentAirfieldId
        };
}