using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record CorrectTotalFlightCount(
    int FlightCount) : ICommand;

public class CorrectTotalFlightCountHandler : ICommandHandler<Aircraft, CorrectTotalFlightCount>
{
    public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, CorrectTotalFlightCount command)
    {
        context.StageEvent(new TotalFlightCountCorrected(DateTimeOffset.UtcNow, command.FlightCount));
        
        return Result.Ok();
    }
}
