using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetArrival(
    DateTime Time,
    string AirfieldId) : ICommand;

public class SetArrivalHandler : ICommandHandler<Flight, SetArrival>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Flight> context, SetArrival command)
    {
        if (!context.Aggregate.IsInitialized) 
            context.WithResult(Result.Fail("Flight does not exist"));
        
        if (string.IsNullOrWhiteSpace(command.AirfieldId))
            context.WithResult(Result.Fail("Airfield was not provided"));

        if (command.Time == DateTime.MinValue
            || command.Time == DateTime.MaxValue)
            context.WithResult(Result.Fail("Arrival time was not provided"));

        
        context.StageEvent(new ArrivalTimeSet(command.Time));
        context.StageEvent(new ArrivalAirfieldSet(command.AirfieldId));
    }
}