using System.Collections.Generic;
using System.Linq;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size)
        {
            return source
                .Select((s, i) => source.Skip(i * size).Take(size))
                .Where(a => a.Any());
        }
    }
}
