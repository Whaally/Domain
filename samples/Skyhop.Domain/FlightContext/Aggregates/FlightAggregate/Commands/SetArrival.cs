using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetArrival(
    DateTime Time,
    Guid AirfieldId) : ICommand;

[GenerateMetadata]
public partial class SetArrivalHandler : ICommandHandler<Flight, SetArrival>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Flight> context, SetArrival command)
    {
        var result = new Command();
        
        if (!context.Aggregate.IsInitialized) 
            result = result.WithError("Flight does not exist");
        
        if (command.AirfieldId == Guid.Empty)
            result = result.WithError("Airfield was not provided");

        if (command.Time == DateTime.MinValue
            || command.Time == DateTime.MaxValue)
            result = result.WithError("Arrival time was not provided");

        
        result.Stage(new ArrivalTimeSet(command.Time));
        result.Stage(new ArrivalAirfieldSet(command.AirfieldId));

        return result;
    }
}