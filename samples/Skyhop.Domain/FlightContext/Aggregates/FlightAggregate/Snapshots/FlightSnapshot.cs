using Whaally.Domain.Abstractions.Aggregate;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots
{
    public record FlightSnapshot(
        string? AircraftId,
        DateTime? DepartureTime,
        DateTime? ArrivalTime) : ISnapshot;

    public class FlightSnapshotFactory : ISnapshotFactory<Flight, FlightSnapshot>
    {
        public FlightSnapshot Instantiate(Flight aggregate)
            => new FlightSnapshot(
                AircraftId: aggregate.AircraftId,
                DepartureTime: aggregate.DepartureTime,
                ArrivalTime: aggregate.ArrivalTime
            );
    }
}
