using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record RemoveAircraft : ICommand;

public class RemoveAircraftHandler : ICommandHandler<Flight, RemoveAircraft>
{
    public void Evaluate(ICommandHandlerContext<Flight> context, RemoveAircraft command)
    {
        if (!context.Aggregate.IsInitialized)
        {
            context.WithResult(Result.Fail("Flight does not exist"));
            return;
        }

        if (string.IsNullOrWhiteSpace(context.Aggregate.AircraftId))
        {
            context.WithResult(Result.Fail("There is no aircraft to remove"));
            return;
        }   
        
        context.StageEvent(new AircraftRemoved(context.Aggregate.AircraftId!));
    }
}
