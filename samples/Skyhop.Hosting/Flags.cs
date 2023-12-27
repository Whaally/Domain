namespace Skyhop.Hosting;

public record Flags
{
    public bool DisableProjectionDaemon { get; init; } = false;
}