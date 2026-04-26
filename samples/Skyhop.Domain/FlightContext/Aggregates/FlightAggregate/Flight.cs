using System.ComponentModel.DataAnnotations;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;

[GenerateMetadata]
public record Flight : IAggregate
{
    public bool IsInitialized { get; init; } = false;

    public Guid AircraftId { get; init; } = Guid.Empty;
    public Guid DepartureAirfieldId { get; init; } = Guid.Empty;
    public Guid ArrivalAirfieldId { get; init; } = Guid.Empty;

    public string? AircraftRegistration { get; init; }

    public DateTime? DepartureTime { get; init; }
    public string? DepartureAirfield { get; init; }

    public DateTime? ArrivalTime { get; init; }
    public string? ArrivalAirfield { get; init; }

    internal List<IEvent> Events { get; init; } = new();
}