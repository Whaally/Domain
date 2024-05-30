using FluentAssertions;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Event;

namespace Skyhop.Domain.Tests.AircraftContext.Aggregates.Commands;

public abstract class RemoveFlightTest(Aircraft aggregate, RemoveFlight command) 
    : SkyhopCommandTest<Aircraft, RemoveFlight>(new RemoveFlightHandler(), aggregate, command)
{
    public class FromCleanSlate() : RemoveFlightTest(new Aircraft(), new RemoveFlight(""))
    {
        [Fact]
        public void Fails() => Result.IsFailed.Should().BeTrue();
    }

    public class RemoveExistingFlight() : RemoveFlightTest(
        new Aircraft
        {
            Flights = new()
            {
                { "1", ( Departure: default, Arrival: default )}
            }
        },
        new RemoveFlight("1"))
    {
        [Fact]
        public void Succeeds() => Result.IsSuccess.Should().BeTrue();

        [Fact]
        public void WithEvent() => Context.Events.Should().ContainItemsAssignableTo<EventEnvelope<FlightRemoved>>();

        [Fact]
        public void EventHasFlightId() => ((EventEnvelope<FlightRemoved>)Context.Events.Single())
            .Message.FlightId.Should().Be(Command.FlightId);
    }
}
