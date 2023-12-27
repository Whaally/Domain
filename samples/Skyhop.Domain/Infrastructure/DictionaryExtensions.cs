namespace Skyhop.Domain.Infrastructure;

internal static class DictionaryExtensions
{
    public static void InsertOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> setter)
        where TValue : new()
    {
        if (!dictionary.Remove(key, out var value))
            value = new();

        dictionary.Add(key, setter(value));
    }
}