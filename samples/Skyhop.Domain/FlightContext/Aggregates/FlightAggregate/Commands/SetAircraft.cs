using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands
{
    public record SetAircraft(
        string AggregateId,
        string AircraftId) : ICommand;

    internal class SetAircraftHandler : ICommandHandler<Flight, SetAircraft>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Flight> context, SetAircraft command)
        {
            var result = new Result();

            if (string.IsNullOrWhiteSpace(command.AircraftId))
                result.WithError("Aircraft was not provided");

            if (result.IsSuccess)
            {
                if (!string.IsNullOrWhiteSpace(context.Aggregate.AircraftId))
                {
                    context.StageEvent(
                        new AircraftRemoved(
                            command.AggregateId, 
                            context.Aggregate.AircraftId));
                }

                context.StageEvent(
                    new AircraftSet(command.AggregateId, command.AircraftId));
            }

            return result;
        }
    }
}
