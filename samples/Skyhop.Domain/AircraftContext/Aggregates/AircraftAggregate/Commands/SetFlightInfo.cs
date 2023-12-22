using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands
{
    [Immutable]
    [GenerateSerializer]
    public record SetFlightInfo(
        string FlightId,
        DateTime? Departure,
        DateTime? Arrival) : ICommand;

    public class SetFlightInfoHandler : ICommandHandler<Aircraft, SetFlightInfo>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, SetFlightInfo command)
        {
            context.StageEvent(new FlightInfoSet(
                command.FlightId,
                command.Departure,
                command.Arrival));

            return Result.Ok();
        }
    }
}
