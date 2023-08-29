using Whaally.Domain.Abstractions.Aggregate;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate
{
    public record Flight : IAggregate
    {
        public string? AircraftId { get; init; }
        public string? DepartureAirfieldId { get; init; }
        public string? ArrivalAirfieldId { get; init; }


        public string? AircraftRegistration { get; init; }

        public DateTime? DepartureTime { get; init; }
        public string? DepartureAirfield { get; init; }

        public DateTime? ArrivalTime { get; init; }
        public string? ArrivalAirfield { get; init; }
    }
}
