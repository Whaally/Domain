using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetDeparture(
    DateTime Time,
    string AirfieldId) : ICommand;

public class SetDepartureHandler : ICommandHandler<Flight, SetDeparture>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Flight> context, SetDeparture command)
    {
        var result = Command.Ok();
        
        if (!context.Aggregate.IsInitialized) 
            result.Bind(() => Command.Fail("Flight does not exist"));
        
        if (string.IsNullOrWhiteSpace(command.AirfieldId))
            result.Bind(() => Command.Fail("Airfield was not provided"));

        if (command.Time == DateTime.MinValue
            || command.Time == DateTime.MaxValue)
            result.Bind(() => Command.Fail("Departure time was not provided"));

        result.Stage(new DepartureTimeSet(command.Time));
        result.Stage(new DepartureAirfieldSet(command.AirfieldId));

        return result;
    }
}
