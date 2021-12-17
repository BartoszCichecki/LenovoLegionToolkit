using System.Collections;

namespace System.Windows.Controls
{
    public static class ItemCollectionExtensions
    {

        public static void AddRange(this ItemCollection itemCollection, IEnumerable enumerable)
        {
            foreach (var item in enumerable)
                itemCollection.Add(item);
        }

        public static void AddEnumValues<T>(this ItemCollection itemCollection) where T : struct, IConvertible
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException($"{type.Name} is not an enumerated type");
            itemCollection.AddRange(Enum.GetValues(type));
        }
    }
}
