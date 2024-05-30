using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain.Abstractions.Saga;

namespace Skyhop.Domain.FlightContext.Sagas;

/*
 * Placement of this saga is tricky.
 *
 * While it responds to a change on the FlightAggregate, the change it triggers is on the AircraftAggregate.
 * Which of the contexts is then responsible for holding this saga?
 *
 * Do we make upstream responsible for notifying downstream, or can downstream subscribe to whatever events they need?
 * Decisions like these impact the way people interact with this codebase, and therefore should be thought about.
 *
 * Naming is also important here, as depending on the context it can imply different things.
 *
 * If we'd move this saga to the AircraftContext, it'd better be named something like "OnFlightsDeclaredAircraftChange"
 * or something.
 *
 * Interestingly the saga doesn't have anything to do with the FlightContext whatsoever, so for this reason it would fit
 * better with the AircraftContext.
 */


public class OnAircraftChanged : ISaga<AircraftSet>
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
