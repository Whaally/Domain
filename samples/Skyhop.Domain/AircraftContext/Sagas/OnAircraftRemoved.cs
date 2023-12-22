using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Saga;

namespace Skyhop.Domain.AircraftContext.Sagas
{
    public class OnAircraftRemoved : ISaga<AircraftRemoved>
    {
        public Task<IResultBase> Evaluate(ISagaContext context, AircraftRemoved @event)
        {
            context.StageCommand(
                @event.AircraftId,
                new RemoveFlight(
                    context.AggregateId!));

            return Task.FromResult<IResultBase>(Result.Ok());
        }
    }
}
