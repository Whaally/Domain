using FluentResults;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.AircraftContext.Sagas;

[GenerateMetadata]
internal partial class OnArrival : ISaga<ArrivalTimeSet>
{
    public async Task<ISagaResult> Evaluate(ISagaContext context, ArrivalTimeSet @event)
    {
        var snapshot = await context.Factory
            .Instantiate<Flight>(context.AggregateId!)
            .Snapshot<FlightSnapshot>();

        if (snapshot.AircraftId is Guid g)
            new Saga().Stage(
                g,
                new SetFlightInfo(
                    g,
                    snapshot.DepartureTime,
                    @event.ArrivalTime));

        return new Saga();
    }
}