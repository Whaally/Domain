using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain.Abstractions.Saga;

namespace Skyhop.Domain.AircraftContext.Sagas;

public class OnDeparture : ISaga<DepartureTimeSet>
{
    public async Task<IResultBase> Evaluate(ISagaContext context, DepartureTimeSet @event)
    {
        var snapshot = await context.Factory
            .Instantiate<Flight>(context.AggregateId!)
            .Snapshot<FlightSnapshot>();

        if (!string.IsNullOrWhiteSpace(snapshot.AircraftId)
            && snapshot.AircraftId != null)
            context.StageCommand(
                snapshot.AircraftId!,
                new SetFlightInfo(
                    context.AggregateId!,
                    @event.DepartureTime,
                    snapshot.ArrivalTime));

        return Result.Ok();
    }
}