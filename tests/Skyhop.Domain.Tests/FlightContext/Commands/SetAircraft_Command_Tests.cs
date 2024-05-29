using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class SetAircraft_Command_Tests : DomainTest
{
    string _flightId = Guid.NewGuid().ToString();
    string _aircraftId = Guid.NewGuid().ToString();

    [Fact]
    public async Task Test_AircraftSet()
    {
        var flight = AggregateFactory.Instantiate<Flight>(_flightId);

        var result = await flight.EvaluateAndApply(
            new Create(),
            new SetAircraft(_aircraftId));
        
        Assert.Empty(result.Errors);
        Assert.Equal(2, result.Value.Length);
        Assert.IsAssignableFrom<IEventEnvelope<AircraftSet>>(result.Value[1]);
    }

    [Fact]
    public async Task AircraftSet_Requires_Aircraft_Id()
    {
        var aggregate = AggregateFactory.Instantiate<Flight>(_flightId);

        var result = await aggregate.EvaluateAndApply(
            new Create(),
            new SetAircraft(""));

        Assert.Single(result.Errors);
    }
}