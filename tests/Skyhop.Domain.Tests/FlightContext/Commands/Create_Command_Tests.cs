using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Event;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class Create_Command_Tests : DomainTest
{
    [Fact]
    public async Task Create_Command_Returns_Created_Event()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");

        var result = await flight.Evaluate(new Create());

        Assert.Empty(result.Errors);
        Assert.IsType<EventEnvelope<Created>>(result.Value.Single());
    }

    [Fact]
    public async Task Create_Command_Cannot_Be_Ran_Twice()
    {
        var flight = AggregateFactory.Instantiate<Flight>("");

        // ToDo: We'll need an `EvaluateAndApply` method to do stuff like this?
        await flight.Apply(
            (await flight.Evaluate(new Create())).Value);

        var result = await flight.Evaluate(new Create());

        Assert.Single(result.Errors);
    }
}
