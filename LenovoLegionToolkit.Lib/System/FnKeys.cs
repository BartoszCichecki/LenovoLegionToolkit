using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.System;

public class FnKeys : SoftwareDisabler
{
    protected override string[] ScheduledTasksPaths => Array.Empty<string>();
    protected override string[] ServiceNames => new[] { "LenovoFnAndFunctionKeys" };
    protected override string[] ProcessNames => new[] { "LenovoUtilityUI", "LenovoUtilityService", "LenovoSmartKey" };

    public override async Task EnableAsync()
    {
        await base.EnableAsync().ConfigureAwait(false);
        SetUwpStartup("LenovoUtility", "LenovoUtilityID", true);
    }

    public override async Task DisableAsync()
    {
        await base.DisableAsync().ConfigureAwait(false);
        SetUwpStartup("LenovoUtility", "LenovoUtilityID", false);
    }

    protected override bool AreProcessesRunning()
    {
        var result = base.AreProcessesRunning();

        if (result)
            return true;

        try
        {
            foreach (var process in Process.GetProcessesByName("utility"))
            {
                var description = process.MainModule?.FileVersionInfo.FileDescription;
                if (description is null)
                    continue;

                if (description.Equals("Lenovo Hotkeys", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
        }
        catch
        {
        }

        return false;
    }

    protected override async Task KillProcessesAsync()
    {
        await base.KillProcessesAsync().ConfigureAwait(false);

        try
        {
            foreach (var process in Process.GetProcessesByName("utility"))
            {
                var description = process.MainModule?.FileVersionInfo.FileDescription;
                if (description is null)
                    continue;

                if (!description.Equals("Lenovo Hotkeys", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                process.Kill();
                await process.WaitForExitAsync().ConfigureAwait(false);
            }
        }
        catch
        {
        }
    }

    private static void SetUwpStartup(string appPattern, string subKeyName, bool enabled)
    {
        const string hive = "HKEY_CURRENT_USER";
        const string subKey = @"Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\SystemAppData";
        const string valueName = "State";

        var startupKey = Registry.GetSubKeys(hive, subKey).FirstOrDefault(s => s.Contains(appPattern, StringComparison.CurrentCultureIgnoreCase));
        if (startupKey is null)
            return;

        Registry.SetValue(hive, startupKey, valueName, enabled ? 0x2 : 0x1);
    }
}