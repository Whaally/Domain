using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Sagas;

internal class OnArrival : ISaga<ArrivalTimeSet>
{
    public async IAsyncEnumerable<IReason> Evaluate(ISagaContext context, ArrivalTimeSet @event)
    {
        var snapshot = await context.Factory
            .Instantiate<Flight>(context.AggregateId!)
            .Snapshot<FlightSnapshot>();

        if (!string.IsNullOrWhiteSpace(snapshot.AircraftId)
            && snapshot.AircraftId != null)
            context.StageCommands(
                snapshot.AircraftId!,
                new SetFlightInfo(
                    context.AggregateId!,
                    snapshot.DepartureTime,
                    @event.ArrivalTime));

        yield break;
    }
}