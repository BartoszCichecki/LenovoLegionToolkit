using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class DictionaryExtensions
{
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> source) where TKey : notnull
    {
        return new ReadOnlyDictionary<TKey, TValue>(source);
    }

    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> items)
    {
        foreach (var keyValuePair in items)
            source.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public static TValue? GetValueOrNull<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
    {
        return !dictionary.TryGetValue(key, out var obj) ? null : obj;
    }
}