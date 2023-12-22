using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands
{
    [Immutable]
    [GenerateSerializer]
    internal record RemoveFlight(string FlightId) : ICommand;

    internal class RemoveFlightHandler : ICommandHandler<Aircraft, RemoveFlight>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, RemoveFlight command)
        {
            context.StageEvent(new FlightRemoved(
                command.FlightId));

            return Result.Ok();
        }
    }
}
