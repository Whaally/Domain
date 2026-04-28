using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class SetAircraft_Command_Tests : DomainTest
{
    Guid _flightId = Guid.NewGuid();
    Guid _aircraftId = Guid.NewGuid();

    [Fact]
    public async Task Test_AircraftSet()
    {
        var flight = AggregateFactory.Instantiate<Flight>(_flightId);

        var result = await flight.Trigger(
            new Create(),
            new SetAircraft(_aircraftId));
        
        Assert.Empty(result.Errors);
        Assert.Equal(2, result.Value?.Messages.Count());
        Assert.IsAssignableFrom<EventEnvelope>(result.Value);
    }

    [Fact]
    public async Task AircraftSet_Requires_Aircraft_Id()
    {
        var aggregate = AggregateFactory.Instantiate<Flight>(_flightId);

        var result = await aggregate.Trigger(
            new Create(),
            new SetAircraft(Guid.Empty));

        Assert.Single(result.Errors);
    }
}