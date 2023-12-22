using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain.Abstractions.Saga;

namespace Skyhop.Domain.AircraftContext.Sagas
{
    internal class OnAircraftChanged : ISaga<AircraftSet>
    {
        public async Task<IResultBase> Evaluate(ISagaContext context, AircraftSet @event)
        {
            var snapshot = await context.Factory
                .Instantiate<Flight>(context.AggregateId!)
                .Snapshot<FlightSnapshot>();

            context.StageCommand(
                @event.AircraftId,
                new SetFlightInfo(
                    context.AggregateId!,
                    snapshot.DepartureTime,
                    snapshot.ArrivalTime));

            return Result.Ok();
        }
    }
}
