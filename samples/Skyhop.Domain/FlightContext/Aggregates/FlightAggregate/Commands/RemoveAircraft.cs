using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record RemoveAircraft : ICommand;

[GenerateMetadata]
public partial class RemoveAircraftHandler : ICommandHandler<Flight, RemoveAircraft>
{
    public async Task<ICommandResult> Evaluate(ICommandHandlerContext<Flight> context, RemoveAircraft command)
    {
        var result = Command.Result;
        
        if (!context.Aggregate.IsInitialized)
            return result.WithError("Flight does not exist");

        if (context.Aggregate.AircraftId == Guid.Empty)
            return result.WithError("There is no aircraft to remove");
        
        return result.Stage(new AircraftRemoved(context.Aggregate.AircraftId));
    }
}
