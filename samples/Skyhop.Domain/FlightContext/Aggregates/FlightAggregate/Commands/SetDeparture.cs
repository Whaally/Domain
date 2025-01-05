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
    public IEnumerable<IReason> Evaluate(ICommandHandlerContext<Flight> context, SetDeparture command)
    {
        if (!context.Aggregate.IsInitialized) 
            yield return new Error("Flight does not exist");
        
        if (string.IsNullOrWhiteSpace(command.AirfieldId))
            yield return new Error("Airfield was not provided");

        if (command.Time == DateTime.MinValue
            || command.Time == DateTime.MaxValue)
            yield return new Error("Departure time was not provided");

        context.StageEvent(
            new DepartureTimeSet(command.Time));
        context.StageEvent(
            new DepartureAirfieldSet(command.AirfieldId));
    }
}