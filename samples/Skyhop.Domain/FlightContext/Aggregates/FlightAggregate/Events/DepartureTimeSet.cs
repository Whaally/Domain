using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

/// <summary>
///     Signals the departure time had been set
/// </summary>
/// <param name="DepartureTime">Time of departure</param>
[Immutable]
[GenerateSerializer]
public record DepartureTimeSet(DateTime DepartureTime) : IEvent;

[GenerateMetadata]
public partial class DepartureTimeSetHandler : IEventHandler<Flight, DepartureTimeSet>
{
    public Flight Apply(IEventHandlerContext<Flight> context, DepartureTimeSet @event) =>
        context.Aggregate with
        {
            DepartureTime = @event.DepartureTime
        };
}