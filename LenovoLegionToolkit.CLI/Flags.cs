using System.Collections.Generic;
using System.Linq;

namespace LenovoLegionToolkit.CLI;

public class Flags
{
    public bool Silent { get; }
    public string? QuickActionRunName { get; }
    public bool Help { get; }

    public Flags(IEnumerable<string> startupArgs)
    {
        var args = startupArgs.ToArray();

        QuickActionRunName = StringValue(args, "--quickAction") ?? StringValue(args, "-qa");
        Silent = BoolValue(args, "--silent") || BoolValue(args, "-s");
        Help = BoolValue(args, "--help") || BoolValue(args, "-h");
    }

    private static bool BoolValue(IEnumerable<string> values, string key) => values.Contains(key);

    private static string? StringValue(IEnumerable<string> values, string key)
    {
        var value = values.FirstOrDefault(s => s.StartsWith(key));
        return value?.Remove(0, key.Length + 1);
    }
}
