namespace Whaally.Domain.Analyzers;

public record ObjectMeta(string Name, string Namespace)
{
    public string Name { get; } = Name;
    public string Namespace { get; } = Namespace;
}
