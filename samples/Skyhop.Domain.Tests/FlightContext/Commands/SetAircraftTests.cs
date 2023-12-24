using Microsoft.Extensions.DependencyInjection;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class SetAircraftTests : DomainTest
{
    string _flightId = Guid.NewGuid().ToString();
    string _aircraftId = Guid.NewGuid().ToString();

    IAggregateHandlerFactory _factory => Services.GetRequiredService<IAggregateHandlerFactory>();

    [Fact]
    public async Task Test_AircraftSet()
    {
            var aggregate = _factory.Instantiate<Flight>(_flightId.ToString());

            var command = new SetAircraft(_aircraftId);

            var result = await aggregate.Evaluate(command);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            Assert.IsAssignableFrom<IEventEnvelope<AircraftSet>>(result.Value[0]);
        }

    [Fact]
    public async Task Test_AircraftSet_FailsWithoutAircraft()
    {
            var aggregate = _factory.Instantiate<Flight>(_flightId.ToString());

            var command = new SetAircraft("");

            var result = await aggregate.Evaluate(command);

            Assert.True(result.IsFailed);
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }
}