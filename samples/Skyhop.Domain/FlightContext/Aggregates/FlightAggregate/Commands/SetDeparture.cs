using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetDeparture(
    DateTime Time,
    string AirfieldId) : ICommand;

public class SetDepartureHandler : ICommandHandler<Flight, SetDeparture>
{
    public void Evaluate(ICommandHandlerContext<Flight> context, SetDeparture command)
    {
        if (!context.Aggregate.IsInitialized) 
            context.WithResult(Result.Fail("Flight does not exist"));
        
        if (string.IsNullOrWhiteSpace(command.AirfieldId))
            context.WithResult(Result.Fail("Airfield was not provided"));

        if (command.Time == DateTime.MinValue
            || command.Time == DateTime.MaxValue)
            context.WithResult(Result.Fail("Departure time was not provided"));

        context.StageEvent(
            new DepartureTimeSet(command.Time));
        context.StageEvent(
            new DepartureAirfieldSet(command.AirfieldId));
    }
}
