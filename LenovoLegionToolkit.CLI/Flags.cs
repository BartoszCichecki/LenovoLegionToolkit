using System.Collections.Generic;
using System.Linq;

namespace LenovoLegionToolkit.CLI;

public class Flags
{
    public bool ShowHelp { get; }
    public bool Silent { get; }
    public string? QuickActionRunName { get; }

    public bool Error { get; }

    public Flags(IEnumerable<string> startupArgs)
    {
        var args = startupArgs.ToArray();
        try
        {
            ShowHelp = BoolValue(args, "--help");
            Silent = BoolValue(args, "--silent");
            QuickActionRunName = StringValue(args, "--quickAction");
        }
        catch
        {
            Error = true;
        }
        
    }

    private static bool BoolValue(IEnumerable<string> values, string key) => values.Contains(key);

    private static string? StringValue(IEnumerable<string> values, string key)
    {
        var value = values.FirstOrDefault(s => s.StartsWith(key));
        return value?.Remove(0, key.Length + 1);
    }
}
