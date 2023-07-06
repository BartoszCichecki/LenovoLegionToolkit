using System;
using System.Collections.Generic;
using System.Linq;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        if (source is ICollection<T> collection)
            return collection.Count == 0;
        return !source.Any();
    }

    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        foreach (var item in enumeration)
            action(item);
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size)
    {
        var enumerable = source as T[] ?? source.ToArray();
        return enumerable
            .Select((_, i) => enumerable.Skip(i * size).Take(size))
            .Where(a => a.Any());
    }
}
