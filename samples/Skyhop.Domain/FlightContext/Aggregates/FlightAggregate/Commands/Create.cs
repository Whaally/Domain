using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record Create() : ICommand;

public class CreateHandler : ICommandHandler<Flight, Create>
{
    public void Evaluate(ICommandHandlerContext<Flight> context, Create command)
    {
        if (context.Aggregate.IsInitialized) 
            context.WithResult(Result.Fail("Flight had already been created"));
        
        context.StageEvent(new Created());
    }
}
