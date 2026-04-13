using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record ArrivalTimeSet(DateTime ArrivalTime) : IEvent;

[GenerateMetadata]
public partial class ArrivalTimeSetHandler : IEventHandler<Flight, ArrivalTimeSet>
{
    public Flight Apply(IEventHandlerContext<Flight> context, ArrivalTimeSet @event) =>
        context.Aggregate with
        {
            ArrivalTime = @event.ArrivalTime
        };
}