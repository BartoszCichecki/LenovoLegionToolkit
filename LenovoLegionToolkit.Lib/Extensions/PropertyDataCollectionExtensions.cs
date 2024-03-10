using System.Management;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class PropertyDataCollectionExtensions
{
    public static bool Contains(this PropertyDataCollection pdc, string name)
    {
        // ReSharper disable once NotDisposedResource
        var enumerator = pdc.GetEnumerator();

        while (enumerator.MoveNext())
            if (enumerator.Current.Name == name)
                return true;

        return false;
    }
}
