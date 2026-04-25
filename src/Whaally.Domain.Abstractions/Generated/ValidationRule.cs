namespace Whaally.Domain.Abstractions.Generated;

public sealed record ValidationRule
{
    public string Name { get; set; } = default!;
    public (string Name, object? Value)[] Arguments { get; set; } = Array.Empty<(string, object?)>();
}