namespace Whaally.Domain.Infrastructure.OrleansMarten.Surrogates
{
    [GenerateSerializer]
    public struct CurrencySurrogate
    {
        [Id(0)] public DateTime? ValidFrom;
        [Id(1)] public DateTime? ValidTo;
        [Id(2)] public string Namespace;
        [Id(3)] public string Code;
        [Id(4)] public double DecimalDigits;
        [Id(5)] public string EnglishName;
        [Id(6)] public string Number;
        [Id(7)] public string Symbol;
    }
}
