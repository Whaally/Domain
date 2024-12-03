using FluentAssertions;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class OnAircraftChanged_Saga_Tests : DomainTest
{
    private readonly string _flightId = Guid.NewGuid().ToString();
    private readonly string _firstAircraftId = Guid.NewGuid().ToString();
    private readonly string _secondAircraftId = Guid.NewGuid().ToString();

    [Fact]
    public async Task EvaluateTest()
    {
        await Domain.Trigger(_flightId, new Create());
        await Domain.Trigger(_flightId, new SetAircraft(_firstAircraftId));
        
        // ToDo: Assert change on a1H but not on a2H
        (await Domain
            .GetAggregate<Aircraft>(_firstAircraftId)
            .Snapshot<AircraftSnapshot>())
            .FlightCount
            .Should().Be(1);
        
        (await Domain
            .GetAggregate<Aircraft>(_secondAircraftId)
            .Snapshot<AircraftSnapshot>())
            .FlightCount
            .Should().Be(0);

        //
        // await fH.EvaluateAndApply(
        //     new SetAircraft(_secondAircraftId));
        //
        // a1HS = await a1H.Snapshot<AircraftSnapshot>();
        // a2HS = await a2H.Snapshot<AircraftSnapshot>();
        //
        // Assert.Equal(0, a1HS.FlightCount);
        // Assert.Equal(1, a2HS.FlightCount);
    }
}