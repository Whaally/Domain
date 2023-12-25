using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class OnAircraftChanged_Saga_Tests : DomainTest
{
    string _flightId = Guid.NewGuid().ToString();
    string _firstAircraftId = Guid.NewGuid().ToString();
    string _secondAircraftId = Guid.NewGuid().ToString();

    [Fact]
    public async Task EvaluateTest()
    {
        var fH = AggregateFactory.Instantiate<Flight>(_flightId);
        var a1H = AggregateFactory.Instantiate<Aircraft>(_firstAircraftId);
        var a2H = AggregateFactory.Instantiate<Aircraft>(_secondAircraftId);

        var c1 = new SetAircraft(_firstAircraftId);

        var events = await fH.Evaluate(c1);

        await fH.Continue(events.Value);

        // ToDo: Assert change on a1H but not on a2H
        var a1HS = await a1H.Snapshot<AircraftSnapshot>();
        var a2HS = await a2H.Snapshot<AircraftSnapshot>();

        Assert.Equal(1, a1HS.FlightCount);
        Assert.Equal(0, a2HS.FlightCount);
            
        var c2 = new SetAircraft(_secondAircraftId);

        await fH.Continue(
            (await fH.Evaluate(c2))
            .Value);

        a1HS = await a1H.Snapshot<AircraftSnapshot>();
        a2HS = await a2H.Snapshot<AircraftSnapshot>();

        Assert.Equal(0, a1HS.FlightCount);
        Assert.Equal(1, a2HS.FlightCount);
    }
}