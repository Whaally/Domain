using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands
{
    internal record RemoveFlight(
        string AggregateId,
        string FlightId) : ICommand;

    internal class RemoveFlightHandler : ICommandHandler<Aircraft, RemoveFlight>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, RemoveFlight command)
        {
            context.StageEvent(new FlightRemoved(
                command.AggregateId,
                command.FlightId));

            return Result.Ok();
        }
    }
}
