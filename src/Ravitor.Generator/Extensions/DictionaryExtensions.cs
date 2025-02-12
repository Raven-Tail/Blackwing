namespace System.Collections.Generic;

internal static class DictionaryExtensions
{
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        (key, value) = (pair.Key, pair.Value);
    }
}
