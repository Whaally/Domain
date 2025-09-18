using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record RemoveAircraft : ICommand;

public class RemoveAircraftHandler : ICommandHandler<Flight, RemoveAircraft>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Flight> context, RemoveAircraft command)
    {
        var result = Command.Ok();
        
        if (!context.Aggregate.IsInitialized)
        {
            result.Bind(() => Command.Fail("Flight does not exist"));
            return result;
        }

        if (string.IsNullOrWhiteSpace(context.Aggregate.AircraftId))
        {
            result.Bind(() => Command.Fail("There is no aircraft to remove"));
            return result;
        }   
        
        return result.Stage(new AircraftRemoved(context.Aggregate.AircraftId!));
    }
}
