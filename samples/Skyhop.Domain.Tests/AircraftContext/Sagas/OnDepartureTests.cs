using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;
using Skyhop.Domain.AircraftContext.Sagas;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain;
using Whaally.Domain.Saga;

namespace Skyhop.Domain.Tests.FlightContext.Commands;

public class OnDepartureTests : DomainTest
{
    [Fact]
    public async Task DepartureTimeSet_Should_Trigger_OnDeparture() {
        var aircraftId = Guid.NewGuid().ToString();
        var flightId = Guid.NewGuid().ToString();
        var departureAirfieldId = Guid.NewGuid().ToString();

        var flight = await AggregateFactory
            .Instantiate<Flight>(flightId)
            .EvaluateAndApply(
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
        var flightId = Guid.NewGuid().ToString();
        
        // First we're instantiating a flight as a snapshot of it will be retrieved by the saga
        await AggregateFactory
            .Instantiate<Flight>(flightId)
            .EvaluateAndApply(
                new Create(),
                new SetAircraft(Guid.NewGuid().ToString()));
        
        // Then we're creating the saga, and instantiating the arguments required for evaluation
        var saga = new OnDeparture();
        
        var context = new SagaContext(Services)
        {
            AggregateId = flightId
        };
        var @event = new DepartureTimeSet(DateTime.Now);

        // Evaluate
        var result = await saga.Evaluate(context, @event);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(context.Commands);
        Assert.IsType<SetFlightInfo>(context.Commands.Single().Message);
    }
}