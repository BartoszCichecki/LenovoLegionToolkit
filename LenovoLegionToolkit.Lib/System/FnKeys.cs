using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.System
{

    public class FnKeys : SoftwareDisabler
    {
        protected override string[] ScheduledTasksPaths => Array.Empty<string>();

        protected override string[] ServiceNames => new[]
        {
            "LenovoFnAndFunctionKeys",
        };

        public override async Task<SoftwareStatus> GetStatusAsync()
        {
            var result = await base.GetStatusAsync().ConfigureAwait(false);

            if (result == SoftwareStatus.Disabled)
            {
                var utilityRunning = Process.GetProcessesByName("utility").Where(p => p.MainModule?.FileName?.Contains("LenovoUtility") ?? false).Any();
                if (utilityRunning)
                    return SoftwareStatus.Enabled;
            }

            return result;
        }

        public override async Task EnableAsync()
        {
            await base.EnableAsync().ConfigureAwait(false);
            Registry.SetUWPStartup("LenovoUtility", "LenovoUtilityID", true);
        }

        public async override Task DisableAsync()
        {
            await base.DisableAsync().ConfigureAwait(false);
            Registry.SetUWPStartup("LenovoUtility", "LenovoUtilityID", false);

            foreach (var process in Process.GetProcessesByName("LenovoSmartKey"))
            {
                process.Kill();
                await process.WaitForExitAsync().ConfigureAwait(false);
            }

            foreach (var process in Process.GetProcessesByName("utility").Where(p => p.MainModule.FileName.Contains("LenovoUtility")))
            {
                process.Kill();
                await process.WaitForExitAsync().ConfigureAwait(false);
            }
        }
    }
}
