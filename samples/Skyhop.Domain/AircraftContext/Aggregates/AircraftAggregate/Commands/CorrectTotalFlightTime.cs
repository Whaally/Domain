using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record CorrectTotalFlightTime(
    TimeSpan TotalTime) : ICommand;

public class CorrectTotalFlightTimeHandler : ICommandHandler<Aircraft, CorrectTotalFlightTime>
{
    public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, CorrectTotalFlightTime command)
    {
        context.StageEvent(new TotalFlightTimeCorrected(DateTimeOffset.UtcNow, command.TotalTime));
        
        return Result.Ok();
    }
}
