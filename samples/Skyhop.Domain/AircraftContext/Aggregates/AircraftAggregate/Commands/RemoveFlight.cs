using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record RemoveFlight(Guid FlightId) : ICommand;

[GenerateMetadata]
public partial class RemoveFlightHandler : ICommandHandler<Aircraft, RemoveFlight>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Aircraft> context, RemoveFlight command)
    {
        return new Command()
            .Stage(new FlightRemoved(command.FlightId));
    }
}