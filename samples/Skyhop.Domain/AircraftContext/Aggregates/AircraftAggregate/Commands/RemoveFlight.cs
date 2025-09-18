using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record RemoveFlight(Guid FlightId) : ICommand;

public class RemoveFlightHandler : ICommandHandler<Aircraft, RemoveFlight>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Aircraft> context, RemoveFlight command)
    {
        return Command.Ok().Stage(new FlightRemoved(command.FlightId));
    }
}