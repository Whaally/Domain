using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Whaally.Domain;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class OnAircraftChanged_Saga_Tests : DomainTest
{
    private readonly string _flightId = Guid.NewGuid().ToString();
    private readonly string _firstAircraftId = Guid.NewGuid().ToString();
    private readonly string _secondAircraftId = Guid.NewGuid().ToString();

    [Fact]
    public async Task EvaluateTest()
    {
        var fH = AggregateFactory.Instantiate<Flight>(_flightId);
        var a1H = AggregateFactory.Instantiate<Aircraft>(_firstAircraftId);
        var a2H = AggregateFactory.Instantiate<Aircraft>(_secondAircraftId);

        await fH.EvaluateAndApply(
            new Create(), 
            new SetAircraft(_firstAircraftId));
        
        // ToDo: Assert change on a1H but not on a2H
        var a1HS = await a1H.Snapshot<AircraftSnapshot>();
        var a2HS = await a2H.Snapshot<AircraftSnapshot>();

        Assert.Equal(1, a1HS.FlightCount);
        Assert.Equal(0, a2HS.FlightCount);

        await fH.EvaluateAndApply(
            new SetAircraft(_secondAircraftId));
        
        a1HS = await a1H.Snapshot<AircraftSnapshot>();
        a2HS = await a2H.Snapshot<AircraftSnapshot>();

        Assert.Equal(0, a1HS.FlightCount);
        Assert.Equal(1, a2HS.FlightCount);
    }
}