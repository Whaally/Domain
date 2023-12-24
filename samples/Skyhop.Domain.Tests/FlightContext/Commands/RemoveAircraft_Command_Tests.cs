using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Event;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class RemoveAircraft_Command_Tests : DomainTest
{
    [Fact]
    public async Task Requires_Flight_To_Be_Iniitalized()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");

        Assert.Single(
            (await flight.Evaluate(new RemoveAircraft()))
            .Errors);
    }

    [Fact]
    public async Task Requires_Aircraft_To_Be_Defined()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");

        await flight.Apply(
            (await flight.Evaluate(new Create())).Value);

        var result = await flight.Evaluate(new RemoveAircraft());

        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task Aircraft_Can_Be_Removed()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");
        
        // ToDo: Allow the evaluation and application of multiple commands in a "one liner".
        await flight.Apply(
            (await flight.Evaluate(new Create())).Value);
        
        await flight.Apply(
            (await flight.Evaluate(new SetAircraftCommand("PH-ABC"))).Value);

        var result = await flight.Evaluate(new RemoveAircraft());

        Assert.Empty(result.Errors);
        Assert.IsType<EventEnvelope<AircraftRemoved>>(result.Value.Single());
    }
}