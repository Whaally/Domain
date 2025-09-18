using FluentResults;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record CorrectTotalFlightTime(
    TimeSpan TotalTime,
    string Reason) : ICommand;

public class CorrectTotalFlightTimeHandler : ICommandHandler<Aircraft, CorrectTotalFlightTime>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Aircraft> context, CorrectTotalFlightTime command)
    {
        return Command.Ok();
    }
}