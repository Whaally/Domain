using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;

[GenerateMetadata]
public record Aircraft : IAggregate
{
    public string? Registration { get; init; }
    public string? Callsign { get; init; }
    public string? Model { get; init; }
    public string? TypeDesignator { get; init; }

    public int Starts { get; init; }
    public TimeSpan FlightTime { get; init; }

    public Dictionary<Guid , (DateTime? Departure, DateTime? Arrival)>
        Flights
    { get; init; } = new();
}