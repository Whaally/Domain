using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;

public record Flight : IAggregate
{
    public bool IsInitialized { get; init; } = false;
    
    public string? AircraftId { get; init; }
    public string? DepartureAirfieldId { get; init; }
    public string? ArrivalAirfieldId { get; init; }

    
    public string? AircraftRegistration { get; init; }

    public DateTime? DepartureTime { get; init; }
    public string? DepartureAirfield { get; init; }

    public DateTime? ArrivalTime { get; init; }
    public string? ArrivalAirfield { get; init; }

    internal List<IEvent> Events { get; init; } = new();
}