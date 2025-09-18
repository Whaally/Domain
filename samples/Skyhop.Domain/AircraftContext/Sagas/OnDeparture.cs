using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Sagas;

public class OnDeparture : ISaga<DepartureTimeSet>
{
    public async Task<ISagaResult> Evaluate(ISagaContext context, DepartureTimeSet @event)
    {
        var snapshot = await context.Factory
            .Instantiate<Flight>(context.AggregateId)
            .Snapshot<FlightSnapshot>();

        if (snapshot.AircraftId is Guid g)
            return Saga
                .Ok()
                .Stage(g!,
                    new SetFlightInfo(
                        g!,
                        @event.DepartureTime,
                        snapshot.ArrivalTime));

        return Saga.Ok();
    }
}