using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class ConfiguredTaskAwaitable
{
    public static ValueTask AsValueTask(this Task task) => new(task);

    public static async Task<T?> OrNullIfException<T>(this ConfiguredTaskAwaitable<T> task) where T : struct
    {
        try { return await task; }
        catch { return null; }
    }
}
