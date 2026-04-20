using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record Create() : ICommand;

[GenerateMetadata]
public partial class CreateHandler : ICommandHandler<Flight, Create>
{
    public ICommandResult Evaluate(ICommandHandlerContext<Flight> context, Create command)
    {
        if (context.Aggregate.IsInitialized) 
            return new Command()
                .WithError("Flight had already been created");
        
        return new Command().Stage(new Created());
    }
}
