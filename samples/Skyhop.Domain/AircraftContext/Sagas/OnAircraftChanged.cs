using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain.Abstractions.Saga;

namespace Skyhop.Domain.AircraftContext.Sagas;

internal class OnAircraftChanged : ISaga<AircraftSet>
{
    public async Task<IResultBase> Evaluate(ISagaContext context, AircraftSet @event)
    {   
        var flight = await context.Factory
            .Instantiate<Flight>(context.AggregateId!)
            .Snapshot<FlightSnapshot>();

        // There is no need to make a change as the flight is up to date with the latest state
        if (flight.AircraftId != @event.AircraftId) return Result.Ok();

        var aircraft = await context.Factory
            .Instantiate<Aircraft>(@event.AircraftId)
            .Snapshot<AircraftSnapshot>();

        if (!aircraft.FlightsIds.Contains(context.AggregateId))
        {
            context.StageCommand(
                @event.AircraftId,
                new SetFlightInfo(
                    context.AggregateId!,
                    flight.DepartureTime,
                    flight.ArrivalTime));
        }

        return Result.Ok();
    }
}
