using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record CorrectTotalFlightCount(
    int FlightCount,
    string Reason) : ICommand;

[Whaally.Domain.Generators.GenerateMetadata]
public partial class CorrectTotalFlightCountHandler : ICommandHandler<Aircraft, CorrectTotalFlightCount>
{
    public async Task<ICommandResult> Evaluate(ICommandHandlerContext<Aircraft> context, CorrectTotalFlightCount command) 
        => Command.Result;
}