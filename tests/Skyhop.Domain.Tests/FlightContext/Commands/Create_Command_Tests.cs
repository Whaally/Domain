using FluentAssertions;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class Create_Command_Tests : DomainTest
{
    [Fact]
    public async Task Create_Command_Returns_Created_Event()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");

        CommandEnvelope<Create> envelope = new Create();
        
        var result = await flight.Evaluate(envelope);

        Assert.Empty(result.Errors);
        Assert.IsType<EventEnvelope<Created>>(result.Value.Single());
    }

    [Fact]
    public async Task Create_Command_Cannot_Be_Ran_Twice()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");

        await flight.Trigger((CommandEnvelope<Create>)new Create());

        var result = await flight.Trigger((CommandEnvelope<Create>)new Create());

        result.Errors.Should().ContainSingle();
    }
}
