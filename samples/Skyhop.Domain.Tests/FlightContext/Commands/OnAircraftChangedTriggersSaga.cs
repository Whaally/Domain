using Microsoft.Extensions.DependencyInjection;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Whaally.Domain;
using Whaally.Domain.Abstractions.Aggregate;

namespace Skyhop.Domain.Tests.FlightContext.Commands
{
    public class OnAircraftChangedTriggersSaga
    {
        string _flightId = Guid.NewGuid().ToString();
        string _firstAircraftId = Guid.NewGuid().ToString();
        string _secondAircraftId = Guid.NewGuid().ToString();

        IServiceProvider _services = new ServiceCollection()
            .AddDomain()
            .BuildServiceProvider();

        IAggregateHandlerFactory _factory => _services.GetRequiredService<IAggregateHandlerFactory>();

        [Fact]
        public async Task EvaluateTest()
        {
            var fH = _factory.Instantiate<Flight>(_flightId);
            var a1H = _factory.Instantiate<Aircraft>(_firstAircraftId);
            var a2H = _factory.Instantiate<Aircraft>(_secondAircraftId);

            var c1 = new SetAircraft(_flightId, _firstAircraftId);

            var events = await fH.Evaluate(c1);

            await fH.Confirm(events.Value);

            // ToDo: Assert change on a1H but not on a2H
            var a1HS = await a1H.Snapshot<AircraftSnapshot>();
            var a2HS = await a2H.Snapshot<AircraftSnapshot>();

            Assert.Equal(1, a1HS.FlightCount);
            Assert.Equal(0, a2HS.FlightCount);
            
            var c2 = new SetAircraft(_flightId, _secondAircraftId);

            await fH.Confirm(
                (await fH.Evaluate(c2))
                .Value);

            a1HS = await a1H.Snapshot<AircraftSnapshot>();
            a2HS = await a2H.Snapshot<AircraftSnapshot>();

            Assert.Equal(0, a1HS.FlightCount);
            Assert.Equal(1, a2HS.FlightCount);
        }
    }
}
