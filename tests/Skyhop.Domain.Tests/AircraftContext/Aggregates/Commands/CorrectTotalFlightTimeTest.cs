using FluentAssertions;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Event;

namespace Skyhop.Domain.Tests.AircraftContext.Aggregates.Commands;

public abstract class CorrectTotalFlightTimeTest(Aircraft aggregate, CorrectTotalFlightTime command) 
    : SkyhopCommandTest<Aircraft, CorrectTotalFlightTime>(new CorrectTotalFlightTimeHandler(), aggregate, command)
{
    public class FromCleanSlate() : CorrectTotalFlightTimeTest(new Aircraft(), new CorrectTotalFlightTime(TimeSpan.Zero))
    {
        [Fact]
        public void Succeeds() => Result.IsSuccess.Should().BeTrue();

        [Fact]
        public void HasEvent() => Context.Events.Should().ContainSingle();

        [Fact]
        public void EventWithType() =>
            Context.Events.Should().ContainItemsAssignableTo<EventEnvelope<TotalFlightTimeCorrected>>();
    }
}