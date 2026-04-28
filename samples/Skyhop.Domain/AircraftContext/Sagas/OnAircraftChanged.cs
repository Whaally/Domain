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
internal partial class OnAircraftChanged : ISaga<AircraftSet>
{
    public async Task<ISagaResult> Evaluate(ISagaContext context, AircraftSet @event)
    {
        var flight = await context.Factory.Instantiate<Flight>(context.AggregateId!);
        var flightSnapshot = await flight.Snapshot<FlightSnapshot>();
        
        // There is no need to make a change as the flight is up to date with the latest state
        if (flightSnapshot.AircraftId != @event.AircraftId) return new SagaResult();
        
        var aircraft = await context.Factory.Instantiate<Aircraft>(@event.AircraftId);
        var aircraftSnapshot = await aircraft.Snapshot<AircraftSnapshot>();
        
        if (!aircraftSnapshot.FlightsIds.Contains(context.AggregateId))
        {
            return new SagaResult()
                .Stage(
                    @event.AircraftId,
                    new SetFlightInfo(
                        context.AggregateId!,
                        flightSnapshot.DepartureTime,
                        flightSnapshot.ArrivalTime));
        }
        
        return new SagaResult();
    }
}
