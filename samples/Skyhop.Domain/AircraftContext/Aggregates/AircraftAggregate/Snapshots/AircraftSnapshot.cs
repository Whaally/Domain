using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots;

public record AircraftSnapshot(
    int FlightCount,
    IEnumerable<Guid> FlightsIds) : ISnapshot;

public class AircraftSnapshotFactory : ISnapshotFactory<Aircraft, AircraftSnapshot>
{
    public AircraftSnapshot Instantiate(Aircraft aggregate) 
        => new(
            aggregate.Flights.Count, 
            aggregate.Flights.Select(q => q.Key));
}