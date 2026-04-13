using FluentResults;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record CorrectTotalFlightTime(
    TimeSpan TotalTime,
    string Reason) : ICommand;

[GenerateMetadata]
public partial class CorrectTotalFlightTimeHandler : ICommandHandler<Aircraft, CorrectTotalFlightTime>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Aircraft> context, CorrectTotalFlightTime command)
    {
        return new Command();
    }
}