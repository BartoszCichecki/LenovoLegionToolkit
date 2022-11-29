using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class DictionaryExtensions
{
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> items)
    {
        foreach (var keyValuePair in items)
            source.Add(keyValuePair.Key, keyValuePair.Value);
    }
}