using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.AircraftContext.Sagas;

[GenerateMetadata]
public partial class OnDeparture : ISaga<DepartureTimeSet>
{
    public async Task<ISagaResult> Evaluate(ISagaContext context, DepartureTimeSet @event)
    {
        var flight = await context.Factory
            .Instantiate<Flight>(context.AggregateId);
        var snapshot = await flight.Snapshot<FlightSnapshot>();

        if (snapshot.AircraftId is { } g)
            return new SagaResult()
                .Stage(g!,
                    new SetFlightInfo(
                        g!,
                        @event.DepartureTime,
                        snapshot.ArrivalTime));

        return new SagaResult();
    }
}