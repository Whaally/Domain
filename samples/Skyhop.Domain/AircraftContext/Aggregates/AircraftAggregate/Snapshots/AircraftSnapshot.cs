using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Whaally.Domain.Abstractions.Aggregate;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Snapshots
{
    public record AircraftSnapshot(int FlightCount) : ISnapshot;

    public class AircraftSnapshotFactory : ISnapshotFactory<Aircraft, AircraftSnapshot>
    {
        public AircraftSnapshot Instantiate(Aircraft aggregate)
            => new AircraftSnapshot(aggregate.Flights.Count);
    }
}
