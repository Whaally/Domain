using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record RemoveAircraft : ICommand;

public class RemoveAircraftHandler : ICommandHandler<Flight, RemoveAircraft>
{
    public IEnumerable<IReason> Evaluate(ICommandHandlerContext<Flight> context, RemoveAircraft command)
    {
        if (!context.Aggregate.IsInitialized) 
            return [ new Error("Flight does not exist") ];
        if (string.IsNullOrWhiteSpace(context.Aggregate.AircraftId))
            return [ new Error("There is no aircraft to remove") ];   
        
        context.StageEvent(new AircraftRemoved(context.Aggregate.AircraftId!));

        return [];
    }
}
