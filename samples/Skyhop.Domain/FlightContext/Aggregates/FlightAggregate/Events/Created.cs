using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;

[Immutable]
[GenerateSerializer]
public record Created() : IEvent;

[GenerateMetadata]
public partial class CreatedHandler : IEventHandler<Flight, Created>
{
    public Flight Apply(IEventHandlerContext<Flight> context, Created @event) =>
        context.Aggregate with
        {
            IsInitialized = true
        };
}