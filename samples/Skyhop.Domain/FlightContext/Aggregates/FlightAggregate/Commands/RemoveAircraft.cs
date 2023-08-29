using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands
{
    public record RemoveAircraft(string AggregateId) : ICommand;

    public class RemoveAircraftHandler : ICommandHandler<Flight, RemoveAircraft>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Flight> context, RemoveAircraft command)
        {
            context.StageEvent(
                new AircraftRemoved(
                    command.AggregateId, 
                    context.Aggregate.AircraftId!));

            return Result.Ok();
        }
    }
}
