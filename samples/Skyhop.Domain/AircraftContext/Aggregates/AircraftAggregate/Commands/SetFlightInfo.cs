using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetFlightInfo(
    Guid FlightId,
    DateTime? Departure,
    DateTime? Arrival) : ICommand;

[GenerateMetadata]
public partial class SetFlightInfoHandler : ICommandHandler<Aircraft, SetFlightInfo>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Aircraft> context, SetFlightInfo command)
    {
        return new Command().Stage(
            new FlightInfoSet(command.FlightId, command.Departure, command.Arrival));
    }
}