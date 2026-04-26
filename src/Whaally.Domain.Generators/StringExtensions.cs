namespace Whaally.Domain.Generators;

internal static class StringExtensions
{
    internal static string Indent(this string str) =>
        string.Join(
            "\n",
            str
                .Split('\n')
                .Select(q => $"    {q}"))
            .Substring(4);
}