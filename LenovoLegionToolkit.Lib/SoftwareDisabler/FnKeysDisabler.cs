using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.SoftwareDisabler;

public class FnKeysDisabler : AbstractSoftwareDisabler
{
    protected override IEnumerable<string> ScheduledTasksPaths => [];
    protected override IEnumerable<string> ServiceNames => ["LenovoFnAndFunctionKeys"];
    protected override IEnumerable<string> ProcessNames => ["LenovoUtilityUI", "LenovoUtilityService", "LenovoSmartKey"];

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

    protected override IEnumerable<string> RunningProcesses()
    {
        var result = base.RunningProcesses().ToList();

        try
        {
            foreach (var process in Process.GetProcessesByName("utility"))
            {
                var description = process.MainModule?.FileVersionInfo.FileDescription;
                if (description is null)
                    continue;

                if (description.Equals("Lenovo Hotkeys", StringComparison.InvariantCultureIgnoreCase))
                    result.Add(process.ProcessName);
            }
        }
        catch {  /* Ignored. */ }

        return result;
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
        catch {  /* Ignored. */ }
    }

    private static void SetUwpStartup(string appPattern, string subKeyName, bool enabled)
    {
        const string hive = "HKEY_CURRENT_USER";
        const string subKey = @"Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\SystemAppData";
        const string valueName = "State";

        var startupKey = Registry.GetSubKeys(hive, subKey).FirstOrDefault(s => s.Contains(appPattern, StringComparison.CurrentCultureIgnoreCase));
        if (startupKey is null)
            return;

        startupKey = Path.Combine(startupKey, subKeyName);

        Registry.SetValue(hive, startupKey, valueName, enabled ? 0x2 : 0x1);
    }
}
