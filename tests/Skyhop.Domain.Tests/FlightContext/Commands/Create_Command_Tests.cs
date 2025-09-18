using FluentAssertions;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class Create_Command_Tests : DomainTest
{
    [Fact]
    public async Task Create_Command_Returns_Created_Event()
    {
        var flight = AggregateFactory.Instantiate<Flight>(Guid.Empty);

        var result = await flight.Evaluate(new Create());

        Assert.Empty(result.Errors);
        Assert.IsType<EventEnvelope>(result.Value);
    }

    [Fact]
    public async Task Create_Command_Cannot_Be_Ran_Twice()
    {
        var flight = AggregateFactory.Instantiate<Flight>(Guid.Empty);

        await flight.Trigger(new Create());

        var result = await flight.Trigger(new Create());

        result.Errors.Should().ContainSingle();
    }
}
