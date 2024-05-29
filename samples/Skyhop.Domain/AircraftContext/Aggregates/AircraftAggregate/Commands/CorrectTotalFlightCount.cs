using FluentResults;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record CorrectTotalFlightCount(
    int FlightCount,
    string Reason) : ICommand;

public class CorrectTotalFlightCountHandler : ICommandHandler<Aircraft, CorrectTotalFlightCount>
{
    public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, CorrectTotalFlightCount command)
        => Result.Ok();
}