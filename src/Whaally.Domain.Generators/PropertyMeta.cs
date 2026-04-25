namespace Whaally.Domain.Generators;

public sealed record PropertyMeta(string Name, IEnumerable<ValidationRule> Rules)
{
    public string Name { get; } = Name;

    public IEnumerable<ValidationRule> Rules { get; set; } = Rules;
}
