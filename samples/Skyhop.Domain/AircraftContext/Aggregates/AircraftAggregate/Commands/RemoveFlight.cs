using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record RemoveFlight(string FlightId) : ICommand;

public class RemoveFlightHandler : ICommandHandler<Aircraft, RemoveFlight>
{
    public IResultBase Evaluate(ICommandHandlerContext<Aircraft> context, RemoveFlight command)
    {
        var result = Result.Merge(
            Result.FailIf(context.Aggregate.Flights.All(q => q.Key != command.FlightId), "Aircraft has not flown this flight"));
        
        if (result.IsSuccess)
            context.StageEvent(new FlightRemoved(command.FlightId));

        return result;
    }
}
