using FluentAssertions;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Sagas;
using Whaally.Domain;
using Whaally.Domain.Event;

namespace Skyhop.Domain.Tests.FlightContext.Sagas;

/*
 * This test works slightly different due to the saga itself calling multiple other sagas.
 *
 * To properly test we must instantiate the requested aggregates beforehand, which is why we allow tests to define
 * the initializer, allowing them to define aggregates.
 */
public abstract class OnAircraftChangedTest(Action<Whaally.Domain.Domain> initializer, EventEnvelope<AircraftSet> @event) 
    : SkyhopSagaTest<AircraftSet>(initializer, new OnAircraftChanged(), @event)
{
    public class FromCleanSlate() : OnAircraftChangedTest(
        domain => { },
        new(new AircraftSet("aircraft"), new EventMetadata("flight")))
    {
        [Fact]
        public void Succeeds() => Result.IsSuccess.Should().BeTrue();

        [Fact(Skip = "Missing the infra to set up this test")]
        public void HasSingleCommand() => Context.Commands.Should().ContainSingle();
    }
    
    // WIP figuring out how to best test sagas
    // public class AddsFlightToAircraft() : OnAircraftChangedTest(
    //     domain =>
    //     {
    //         _ = domain.GetAggregate<Flight>("flight")
    //             .Result
    //             .EvaluateAndApply(new Create(), new SetAircraft("aircraft-1"))
    //     },
    //     new(new AircraftSet("aircraft-2"), new EventMetadata("flight")))
    // {
    //     
    // }
}
