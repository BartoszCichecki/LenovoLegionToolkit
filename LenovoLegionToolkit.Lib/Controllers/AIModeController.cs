using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class AIModeController
    {
        private readonly HashSet<int> _runningProcessIds = new();
        private readonly Dictionary<string, int> _subModeData = new();

        private readonly BalanceModeSettings _settings;

        private IDisposable? _startProcessListener;
        private IDisposable? _stopProcessListener;

        public bool IsEnabled
        {
            get => _settings.Store.AIModeEnabled;
            set
            {
                _settings.Store.AIModeEnabled = value;
                _settings.SynchronizeStore();
            }
        }

        public AIModeController(BalanceModeSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task StartStopAsync(PowerModeState powerModeState)
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            var isCompatible = mi.Properties.SupportsIntelligentSubMode;
            if (!isCompatible)
                return;

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
            await LoadSubModesAsync().ConfigureAwait(false);
            await SetSubModeIfNeededAsync().ConfigureAwait(false);

            _startProcessListener = CreateStartProcessListener();
            _stopProcessListener = CreateStopProcessListener();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Started");
        }

        private async Task StopAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopping...");

            _startProcessListener?.Dispose();
            _stopProcessListener?.Dispose();

            _runningProcessIds.Clear();
            _subModeData.Clear();

            await SetSubModeAsync(0).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopped");
        }

        private void ProcessStarted(string processName, int processId)
        {
            var subMode = GetSubMode(processName);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process {processName} started. [processId={processId}, subMode={subMode}]");

            _runningProcessIds.Add(processId);

            Task.Run(() => SetSubModeAsync(subMode));
        }

        private void ProcessStopped(int processId)
        {
            if (!_runningProcessIds.Contains(processId))
                return;

            _runningProcessIds.Remove(processId);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process {processId} stopped.");

            Task.Run(() => SetSubModeAsync(1));
        }

        private int GetSubMode(string processName) => _subModeData.TryGetValue(processName, out var result) ? result : 1;

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

        private async Task LoadSubModesAsync()
        {
            _subModeData.Clear();

            try
            {
                var subModes = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_INTELLIGENT_OP_LIST",
                pdc =>
                {
                    var processName = pdc["processname"].Value.ToString();
                    var mode = Convert.ToInt32(pdc["mode"].Value);
                    return (processName, mode);
                }).ConfigureAwait(false);

                _subModeData.AddRange(subModes.OfType<(string, int)>().ToDictionary(sm => sm.Item1, sm => sm.Item2));

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Loaded {_subModeData.Count} sub modes.");
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to load sub modes.", ex);
            }
        }

        private async Task SetSubModeIfNeededAsync()
        {
            var currentSubMode = 1;

            foreach (var (processName, subMode) in _subModeData)
            {
                var process = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName)).FirstOrDefault();
                if (process is null)
                    continue;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Found running process {processName}. [processId={process.Id}, subMode={subMode}]");

                _runningProcessIds.Add(process.Id);
                currentSubMode = subMode;
                break;
            }

            await SetSubModeAsync(currentSubMode).ConfigureAwait(false);
        }

        private async Task SetSubModeAsync(int subMode)
        {
            try
            {
                await WMI.CallAsync("root\\WMI",
                    $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                    "SetIntelligentSubMode",
                    new() { { "Data", subMode } }).ConfigureAwait(false);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Sub mode set to {subMode}.");
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to set sub mode {subMode}.", ex);
            }
        }
    }
}
