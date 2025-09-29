using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetDeparture(
    DateTime Time,
    Guid AirfieldId) : ICommand;

public class SetDepartureHandler : ICommandHandler<Flight, SetDeparture>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Flight> context, SetDeparture command)
    {
        var result = new Command();
        
        if (!context.Aggregate.IsInitialized) 
            result = result.WithError("Flight does not exist");
        
        if (command.AirfieldId == Guid.Empty)
            result = result.WithError("Airfield was not provided");

        if (command.Time == DateTime.MinValue
            || command.Time == DateTime.MaxValue)
            result = result.WithError("Departure time was not provided");

        result.Stage(new DepartureTimeSet(command.Time));
        result.Stage(new DepartureAirfieldSet(command.AirfieldId));

        return result;
    }
}
