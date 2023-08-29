using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands
{
    internal record SetFlightInfo(
        string AggregateId,
        string FlightId,
        DateTime? Departure,
        DateTime? Arrival) : ICommand;

    internal class SetFlightInfoHandler : ICommandHandler<Aircraft, SetFlightInfo>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, SetFlightInfo command)
        {
            context.StageEvent(new FlightInfoSet(
                command.AggregateId,
                command.FlightId,
                command.Departure,
                command.Arrival));

            return Result.Ok();
        }
    }
}
