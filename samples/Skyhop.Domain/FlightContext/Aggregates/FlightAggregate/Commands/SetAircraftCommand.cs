using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetAircraftCommand(string AircraftId) : ICommand;

public class SetAircraftHandler : ICommandHandler<Flight, SetAircraftCommand>
{
    public IResultBase Evaluate(ICommandHandlerContext<Flight> context, SetAircraftCommand command)
    {
        var result = new Result();

        if (!context.Aggregate.IsInitialized) 
            result.WithError("Flight does not exist");
        
        if (string.IsNullOrWhiteSpace(command.AircraftId))
            result.WithError("Aircraft was not provided");

        if (result.IsSuccess)
        {
            if (!string.IsNullOrWhiteSpace(context.Aggregate.AircraftId))
            {
                context.StageEvent(
                    new AircraftRemoved(
                        context.Aggregate.AircraftId));
            }

            context.StageEvent(
                new AircraftSet(command.AircraftId));
        }

        return result;
    }
}