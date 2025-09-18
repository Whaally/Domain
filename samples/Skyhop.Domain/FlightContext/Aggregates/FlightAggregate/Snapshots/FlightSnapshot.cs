using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;

public record FlightSnapshot(
    Guid? AircraftId,
    DateTime? DepartureTime,
    DateTime? ArrivalTime) : ISnapshot;

public class FlightSnapshotFactory : ISnapshotFactory<Flight, FlightSnapshot>
{
    public FlightSnapshot Instantiate(Flight aggregate) =>
        new(
            AircraftId: aggregate.AircraftId,
            DepartureTime: aggregate.DepartureTime,
            ArrivalTime: aggregate.ArrivalTime
        );
}