namespace Whaally.Domain.Generators;

public sealed record ValidationRule(
    string Name,
    IDictionary<string, object?> Arguments)
{
    public string Name { get; } = Name;
    public IDictionary<string, object?> Arguments { get; } = Arguments;
}
