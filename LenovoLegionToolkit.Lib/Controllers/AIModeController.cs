using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class AIModeController
    {
        private readonly HashSet<int> _runningProcessIds = new();

        private readonly BalanceModeSettings _settings;

        private IDisposable? _startProcessListener;
        private IDisposable? _stopProcessListener;

        private Dictionary<string, int>? _subModes;

        public AIModeController(BalanceModeSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task StartStopAsync(PowerModeState powerModeState)
        {
            if (powerModeState == PowerModeState.Balance && _settings.Store.AIModeEnabled)
                await StartAsync().ConfigureAwait(false);
            else
                await StopAsync().ConfigureAwait(false);
        }

        private async Task StartAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting...");

            await StopAsync().ConfigureAwait(false);

            await LoadSubModes().ConfigureAwait(false);

            _startProcessListener = CreateStartProcessListener();
            _stopProcessListener = CreateStopProcessListener();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Started");
        }

        private Task StopAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopping...");

            _startProcessListener?.Dispose();
            _stopProcessListener?.Dispose();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopped");

            return Task.CompletedTask;
        }

        private void ProcessStarted(string processName, int processId)
        {
            var mode = GetSubMode(processName);
            if (mode < 1)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process {processName} started. [processId={processId}, mode={mode}]");

            _runningProcessIds.Add(processId);

            Task.Run(() => SetSubMode(mode));
        }

        private void ProcessStopped(int processId)
        {
            if (!_runningProcessIds.Contains(processId))
                return;

            _runningProcessIds.Remove(processId);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process {processId} stopped.");

            Task.Run(() => SetSubMode(0));
        }

        private int GetSubMode(string processName) => _subModes?.TryGetValue(processName, out var result) == true ? result : 0;

        private IDisposable CreateStartProcessListener() => WMI.Listen("root\\CIMV2",
            $"SELECT * FROM Win32_ProcessStartTrace",
            pdc =>
            {
                var processName = pdc["ProcessName"].Value.ToString();
                if (!int.TryParse(pdc["ProcessID"].Value?.ToString(), out var processID))
                    processID = 0;

                if (processName is not null && processID > 0)
                    ProcessStarted(processName, processID);
            });

        private IDisposable CreateStopProcessListener() => WMI.Listen("root\\CIMV2",
            $"SELECT * FROM Win32_ProcessStopTrace",
            pdc =>
            {
                if (!int.TryParse(pdc["ProcessID"].Value?.ToString(), out var processId))
                    processId = 0;

                if (processId > 0)
                    ProcessStopped(processId);
            });

        private async Task LoadSubModes()
        {
            if (_subModes is not null)
                return;

            var subModes = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_INTELLIGENT_OP_LIST",
                pdc =>
                {
                    var processName = pdc["processname"].Value.ToString();
                    var mode = Convert.ToInt32(pdc["mode"].Value);
                    return (processName, mode);
                }).ConfigureAwait(false);

            _subModes = new();

            foreach (var (processName, mode) in subModes)
            {
                if (processName is null)
                    continue;
                _subModes[processName] = mode;
            }
        }

        private Task SetSubMode(int mode) => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetIntelligentSubMode",
            new() { { "Data", mode } });
    }
}
