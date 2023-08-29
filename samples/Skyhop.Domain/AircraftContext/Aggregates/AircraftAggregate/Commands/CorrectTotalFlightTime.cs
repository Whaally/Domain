using FluentResults;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands
{
    public record CorrectTotalFlightTime(
        string AggregateId,
        TimeSpan TotalTime,
        string Reason) : ICommand;

    internal class CorrectTotalFlightTimeHandler : ICommandHandler<Aircraft, CorrectTotalFlightTime>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, CorrectTotalFlightTime command)
            => Result.Ok();
    }
}
