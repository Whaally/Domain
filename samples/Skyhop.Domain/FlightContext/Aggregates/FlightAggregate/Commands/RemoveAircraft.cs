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
            return result.Bind(() => Command.Fail("Flight does not exist"));

        if (context.Aggregate.AircraftId == Guid.Empty)
            return result.Bind(() => Command.Fail("There is no aircraft to remove"));
        
        return result.Stage(new AircraftRemoved(context.Aggregate.AircraftId));
    }
}
