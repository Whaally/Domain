using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetAircraft(string AircraftId) : ICommand;

public class SetAircraftHandler : ICommandHandler<Flight, SetAircraft>
{
    public void Evaluate(ICommandHandlerContext<Flight> context, SetAircraft command)
    {
        if (!context.Aggregate.IsInitialized) 
            context.WithResult(Result.Fail("Flight does not exist"));
        
        if (string.IsNullOrWhiteSpace(command.AircraftId))
            context.WithResult(Result.Fail("Aircraft was not provided"));

        
        if (!string.IsNullOrWhiteSpace(context.Aggregate.AircraftId))
        {
            context.StageEvent(
                new AircraftRemoved(
                    context.Aggregate.AircraftId));
        }

        context.StageEvent(
            new AircraftSet(command.AircraftId));
    }
}
