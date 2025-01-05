using FluentResults;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record CorrectTotalFlightTime(
    TimeSpan TotalTime,
    string Reason) : ICommand;

public class CorrectTotalFlightTimeHandler : ICommandHandler<Aircraft, CorrectTotalFlightTime>
{
    public void Evaluate(ICommandHandlerContext<Aircraft> context, CorrectTotalFlightTime command)
    {
        
    }
}