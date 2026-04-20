using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

/// <summary>
///     Signals the departure airfield had been set
/// </summary>
/// <param name="AirfieldId">Id of the departure airfield</param>
[Immutable]
[GenerateSerializer]
public record DepartureAirfieldSet(Guid AirfieldId) : IEvent;

[GenerateMetadata]
public partial class DepartureAirfieldSetHandler : IEventHandler<Flight, DepartureAirfieldSet>
{
    public Flight Apply(IEventHandlerContext<Flight> context, DepartureAirfieldSet @event) =>
        context.Aggregate with
        {
            DepartureAirfieldId = @event.AirfieldId
        };
}