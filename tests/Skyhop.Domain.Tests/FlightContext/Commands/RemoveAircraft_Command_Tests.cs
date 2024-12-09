using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class RemoveAircraft_Command_Tests : DomainTest
{
    [Fact]
    public async Task Requires_Flight_To_Be_Iniitalized()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");

        Assert.Single(
            (await flight.Evaluate((CommandEnvelope<RemoveAircraft>)new RemoveAircraft()))
            .Errors);
    }

    [Fact]
    public async Task Requires_Aircraft_To_Be_Defined()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");

        await flight.Apply(
            (await flight.Evaluate((CommandEnvelope<Create>)new Create())).Value);

        var result = await flight.Evaluate((CommandEnvelope<RemoveAircraft>)new RemoveAircraft());

        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task Aircraft_Can_Be_Removed()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");
        
        await flight.Trigger(
            (CommandEnvelope<Create>)new Create(),
            (CommandEnvelope<SetAircraft>)new SetAircraft(Guid.NewGuid().ToString()));

        var result = await flight.Evaluate((CommandEnvelope<RemoveAircraft>)new RemoveAircraft());

        Assert.Empty(result.Errors);
        Assert.IsType<EventEnvelope<AircraftRemoved>>(result.Value.Single());
    }
}