using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetArrival(
    DateTime Time,
    Guid AirfieldId) : ICommand;

public class SetArrivalHandler : ICommandHandler<Flight, SetArrival>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Flight> context, SetArrival command)
    {
        var result = Command.Ok();
        
        if (!context.Aggregate.IsInitialized) 
            result.Bind(() => Command.Fail("Flight does not exist"));
        
        if (command.AirfieldId == Guid.Empty)
            result.Bind(() => Command.Fail("Airfield was not provided"));

        if (command.Time == DateTime.MinValue
            || command.Time == DateTime.MaxValue)
            result.Bind(() => Command.Fail("Arrival time was not provided"));

        
        result.Stage(new ArrivalTimeSet(command.Time));
        result.Stage(new ArrivalAirfieldSet(command.AirfieldId));

        return result;
    }
}