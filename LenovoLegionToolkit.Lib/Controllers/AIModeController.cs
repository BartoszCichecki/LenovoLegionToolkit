using System;
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
        private struct SubMode
        {
            public string ProcessName { get; init; }
            public int Mode { get; init; }
        }

        private readonly BalanceModeSettings _settings;

        private IDisposable? _startProcessListener;
        private IDisposable? _stopProcessListener;

        private SubMode[]? _subModes;

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

        private void ProcessStarted(string processName)
        {
            var mode = GetSubMode(processName);
            if (mode < 1)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process {processName} started. [mode={mode}]");

            Task.Run(() => SetSubMode(mode));
        }

        private void ProcessStopped(string processName)
        {
            var mode = GetSubMode(processName);
            if (mode < 1)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process {processName} stopped.");

            Task.Run(() => SetSubMode(0));
        }

        private int GetSubMode(string processName)
        {
            if (_subModes is null)
                return 0;

            return _subModes
                .Where(sm => sm.ProcessName.Equals(processName, StringComparison.InvariantCultureIgnoreCase))
                .Select(sm => sm.Mode)
                .FirstOrDefault();
        }

        private IDisposable CreateStartProcessListener() => WMI.Listen("root\\CIMV2",
            $"SELECT * FROM Win32_ProcessStartTrace",
            pdc =>
            {
                var processName = pdc["ProcessName"].ToString();
                if (processName is not null)
                    ProcessStarted(processName);
            });

        private IDisposable CreateStopProcessListener() => WMI.Listen("root\\CIMV2",
            $"SELECT * FROM Win32_ProcessStopTrace",
            pdc =>
            {
                var processName = pdc["ProcessName"].ToString();
                if (processName is not null)
                    ProcessStopped(processName);
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
                    return new SubMode { ProcessName = processName, Mode = mode };
                }).ConfigureAwait(false);

            _subModes = subModes.ToArray();
        }

        private Task SetSubMode(int mode) => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetIntelligentSubMode",
            new() { { "Data", mode } });
    }
}
