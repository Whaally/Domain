using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class RemoveAircraft_Command_Tests : DomainTest
{
    [Fact]
    public async Task Requires_Flight_To_Be_Iniitalized()
    {
        var flight = AggregateFactory.Instantiate<Flight>(Guid.Empty);

        Assert.Single(
            (await flight.Evaluate(
                new CommandEnvelope(
                    new CommandMetadata(), 
                    new RemoveAircraft())))
            .Errors);
    }

    [Fact]
    public async Task Requires_Aircraft_To_Be_Defined()
    {
        var flight = AggregateFactory.Instantiate<Flight>(Guid.Empty);

        await flight.Apply(
            (await flight.Evaluate(new Create())).Value);

        var result = await flight.Evaluate(new RemoveAircraft());

        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task Aircraft_Can_Be_Removed()
    {
        var flight = AggregateFactory.Instantiate<Flight>(Guid.Empty);
        
        await flight.Trigger(
            new Create(),
            new SetAircraft(Guid.NewGuid()));

        var result = await flight.Evaluate(new RemoveAircraft());

        Assert.Empty(result.Errors);
        Assert.IsType<EventEnvelope>(result.Value);
    }
}