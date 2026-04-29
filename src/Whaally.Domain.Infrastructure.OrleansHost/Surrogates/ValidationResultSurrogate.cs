namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct ValidationResultSurrogate
{
    /// <summary>
    ///     Gets the collection of member names affected by this result.  The collection may be empty but will never be null.
    /// </summary>
    [Id(0)]
    public IEnumerable<string> MemberNames;

    /// <summary>
    ///     Gets the error message for this result.  It may be null.
    /// </summary>
    [Id(1)] public string? ErrorMessage;
}