using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.AircraftContext.Sagas;

[GenerateMetadata]
public partial class OnAircraftRemoved : ISaga<AircraftRemoved>
{
    public async Task<ISagaResult> Evaluate(ISagaContext context, AircraftRemoved @event)
    {
        var flight = await context.Factory
            .Instantiate<Flight>(context.AggregateId!)
            .Snapshot<FlightSnapshot>();

        // No need to make a change; nothing to remove here.
        if (flight.AircraftId == @event.AircraftId) return new SagaResult();
        
        var aircraft = await context.Factory
            .Instantiate<Aircraft>(@event.AircraftId)
            .Snapshot<AircraftSnapshot>();
        
        if (aircraft.FlightsIds.Contains(context.AggregateId))
        {
            return new SagaResult().Stage(
                @event.AircraftId,
                new RemoveFlight(
                    context.AggregateId));
        }

        return new SagaResult();
    }
}
