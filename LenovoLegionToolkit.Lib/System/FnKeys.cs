using System;
using System.Diagnostics;
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
        Registry.SetUWPStartup("LenovoUtility", "LenovoUtilityID", true);
    }

    public override async Task DisableAsync()
    {
        await base.DisableAsync().ConfigureAwait(false);
        Registry.SetUWPStartup("LenovoUtility", "LenovoUtilityID", false);
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
}