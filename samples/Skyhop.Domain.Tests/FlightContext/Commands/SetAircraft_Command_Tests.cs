using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class SetAircraft_Command_Tests : DomainTest
{
    string _flightId = Guid.NewGuid().ToString();
    string _aircraftId = Guid.NewGuid().ToString();

    [Fact]
    public async Task Test_AircraftSet()
    {
        var aggregate = AggregateFactory.Instantiate<Flight>(_flightId);

        var result = await aggregate.Evaluate(new SetAircraftCommand(_aircraftId));

        Assert.Empty(result.Errors);
        Assert.IsAssignableFrom<IEventEnvelope<AircraftSet>>(result.Value.Single());
    }

    [Fact]
    public async Task AircraftSet_Requires_Aircraft_Id()
    {
        var aggregate = AggregateFactory.Instantiate<Flight>(_flightId);

        var result = await aggregate.Evaluate(new SetAircraftCommand(""));

        Assert.Single(result.Errors);
    }
}