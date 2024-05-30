using FluentAssertions;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
using Whaally.Domain.Event;

namespace Skyhop.Domain.Tests.AircraftContext.Aggregates.Commands;

public abstract class CorrectTotalFlightCountTest(Aircraft aggregate, CorrectTotalFlightCount command) : SkyhopCommandTest<Aircraft, CorrectTotalFlightCount>(
    new CorrectTotalFlightCountHandler(),
    aggregate,
    command)
{
    public class FromCleanSlate() : CorrectTotalFlightCountTest(new Aircraft(), new CorrectTotalFlightCount(0))
    {
        [Fact]
        public void Succeeds() => Result.IsSuccess.Should().BeTrue();

        [Fact]
        public void HasEvent() => Context.Events.Should().ContainSingle();

        [Fact]
        public void EventOfType() => Context.Events.Should().ContainItemsAssignableTo<EventEnvelope<TotalFlightCountCorrected>>();
    }
}
