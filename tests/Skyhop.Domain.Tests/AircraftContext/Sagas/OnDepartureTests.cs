using FluentAssertions;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.AircraftContext.Sagas;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Timer = System.Timers.Timer;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class OnDepartureTests : DomainTest
{
    [Fact(Skip = "Since sagas are triggered asynchronously, these tests are no longer representative of actual behaviour")]
    public async Task DepartureTimeSet_Should_Trigger_OnDeparture() {
        var aircraftId = Guid.NewGuid();
        var flightId = Guid.NewGuid();
        var departureAirfieldId = Guid.NewGuid();

        await Domain.Invoke(flightId,
            new Create(),
            new SetAircraft(aircraftId),
            new SetDeparture(
                DateTime.UtcNow,
                departureAirfieldId
            ));
        
        var aircraft = await AggregateFactory
            .Instantiate<Aircraft>(aircraftId)
            .Snapshot<AircraftSnapshot>();
        
        Assert.Contains(flightId, aircraft.FlightsIds);
    }

    [Fact]
    public async Task OnDeparture_Should_Stage_SetFlightInfo()
    {
        var flightId = Guid.NewGuid();
        
        // First we're instantiating a flight as a snapshot of it will be retrieved by the saga
        await AggregateFactory
            .Instantiate<Flight>(flightId)
            .Trigger(
                new Create(),
                new SetAircraft(Guid.NewGuid()));
        
        // Then we're creating the saga, and instantiating the arguments required for evaluation
        var saga = new OnDeparture();
        
        var context = new SagaContext(Services, new EventMetadata())
        {
            AggregateId = flightId
        };
        
        var @event = new DepartureTimeSet(DateTime.Now);

        // Evaluate
        var result = await saga.Evaluate(context, @event);
        
        // Assert
        result.Errors.Should().BeEmpty();
        Assert.Single(result.Operations);
        Assert.IsType<SetFlightInfo>(result.Operations.Single().Messages.Single());
    }
}
