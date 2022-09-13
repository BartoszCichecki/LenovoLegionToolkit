using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System
{
    public class LegionZone : SoftwareDisabler
    {
        protected override string[] ScheduledTasksPaths => Array.Empty<string>();

        protected override string[] ServiceNames => new[]
        {
            "LZService"
        };

        public override async Task<SoftwareStatus> GetStatusAsync()
        {
            var status = await base.GetStatusAsync().ConfigureAwait(false);

            if (status == SoftwareStatus.Disabled)
            {
                var lzTrayRunning = Process.GetProcessesByName("LZTray").Any();
                if (lzTrayRunning)
                    status = SoftwareStatus.Enabled;
            }

            return status;
        }

        public override async Task DisableAsync()
        {
            await base.DisableAsync().ConfigureAwait(false);

            foreach (var process in Process.GetProcessesByName("LegionZone"))
            {
                try
                {
                    process.Kill();
                    await process.WaitForExitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Couldn't kill process.", ex);
                }
            }

            foreach (var process in Process.GetProcessesByName("LZTray"))
            {
                try
                {
                    process.Kill();
                    await process.WaitForExitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Couldn't kill process.", ex);
                }
            }

            await Task.Delay(1000).ConfigureAwait(false);
        }
    }
}
