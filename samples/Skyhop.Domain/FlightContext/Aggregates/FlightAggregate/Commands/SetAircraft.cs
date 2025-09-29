using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetAircraft(Guid AircraftId) : ICommand;

public class SetAircraftHandler : ICommandHandler<Flight, SetAircraft>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Flight> context, SetAircraft command)
    {
        var result = new Command();
        if (!context.Aggregate.IsInitialized) 
            result = result.WithError("Flight does not exist");
        
        if (command.AircraftId == Guid.Empty)
            result = result.WithError("Aircraft was not provided");

        
        if (context.Aggregate.AircraftId != Guid.Empty)
        {
            result.Stage(
                new AircraftRemoved(
                    context.Aggregate.AircraftId));
        }

        return result.Stage(new AircraftSet(command.AircraftId));
    }
}
