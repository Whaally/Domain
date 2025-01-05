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
    public void Evaluate(ICommandHandlerContext<Flight> context, SetArrival command)
    {
        if (!context.Aggregate.IsInitialized) 
            context.Result.WithError("Flight does not exist");
        
        if (string.IsNullOrWhiteSpace(command.AirfieldId))
            context.Result.WithError("Airfield was not provided");

        if (command.Time == DateTime.MinValue
            || command.Time == DateTime.MaxValue)
            context.Result.WithError("Arrival time was not provided");

        
        context.StageEvent(new ArrivalTimeSet(command.Time));
        context.StageEvent(new ArrivalAirfieldSet(command.AirfieldId));
    }
}