using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Controllers;

public class AIModeController
{
    private readonly TimeSpan _setInitialDelay = TimeSpan.FromSeconds(3);
    private readonly AsyncLock _startStopLock = new();

    private readonly HashSet<int> _runningProcessIds = new();
    private readonly Dictionary<string, int> _subModeData = new();

    private readonly BalanceModeSettings _settings;

    private CancellationTokenSource? _setInitialDelayedCancellationTokenSource;
    private Task? _setInitialDelayedTask;
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

    public async Task StartAsync(PowerModeState powerModeState)
    {
        using (await _startStopLock.LockAsync().ConfigureAwait(false))
        {
            if (!await IsSupportedAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Not supported.");

                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting...");

            await StopAsync(powerModeState).ConfigureAwait(false);

            _setInitialDelayedCancellationTokenSource?.Cancel();
            if (_setInitialDelayedTask is not null)
                await _setInitialDelayedTask.ConfigureAwait(false);

            if (powerModeState != PowerModeState.Balance || !_settings.Store.AIModeEnabled)
                return;

            await LoadSubModesAsync().ConfigureAwait(false);

            _setInitialDelayedCancellationTokenSource = new CancellationTokenSource();
            _setInitialDelayedTask = SetInitialIntelligentSubModeDelayedAsync(_setInitialDelayedCancellationTokenSource.Token);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Started");
        }
    }

    public async Task StopAsync(PowerModeState powerModeState)
    {
        using (await _startStopLock.LockAsync().ConfigureAwait(false))
        {
            if (!await IsSupportedAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Not supported.");

                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopping...");

            _startProcessListener?.Dispose();
            _stopProcessListener?.Dispose();
            _startProcessListener = null;
            _stopProcessListener = null;

            _runningProcessIds.Clear();
            _subModeData.Clear();

            if (powerModeState == PowerModeState.Balance && await GetIntelligentSubModeAsync().ConfigureAwait(false) != 0)
                await SetIntelligentSubModeAsync(0).ConfigureAwait(false);

            if (_setInitialDelayedTask is not null)
                await _setInitialDelayedTask.ConfigureAwait(false);

            _setInitialDelayedCancellationTokenSource = null;
            _setInitialDelayedTask = null;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopped");
        }
    }

    private async Task<bool> IsSupportedAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        return mi.Properties.SupportsIntelligentSubMode;
    }

    private void ProcessStarted(int processId, string processName)
    {
        var targetSubMode = _subModeData.TryGetValue(processName, out var result) ? result : 1;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Process {processName} started. [processId={processId}, targetSubMode={targetSubMode}]");

        _runningProcessIds.Add(processId);

        Task.Run(async () =>
        {
            var currentSubMode = await GetIntelligentSubModeAsync().ConfigureAwait(false);
            if (currentSubMode != targetSubMode)
                await SetIntelligentSubModeAsync(targetSubMode).ConfigureAwait(false);
        });
    }

    private void ProcessStopped(int processId, string processName)
    {
        if (!_runningProcessIds.Contains(processId))
            return;

        _runningProcessIds.Remove(processId);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Process {processId} stopped.");

        Task.Run(() => SetIntelligentSubModeAsync(1));
    }

    private async Task LoadSubModesAsync()
    {
        _subModeData.Clear();

        try
        {
            var subModes = await WMI.LenovoIntelligentOPList.ReadAsync().ConfigureAwait(false);
            _subModeData.AddRange(subModes.ToDictionary(kv => Path.GetFileNameWithoutExtension(kv.Key), kv => kv.Value));

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Loaded {_subModeData.Count} sub modes.");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to load sub modes.", ex);
        }
    }

    private async Task SetInitialIntelligentSubModeDelayedAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(_setInitialDelay, ct).ConfigureAwait(false);

            var targetSubMode = 1;

            foreach (var (processName, subMode) in _subModeData)
            {
                var process = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName)).FirstOrDefault();
                if (process is null)
                    continue;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Found running process {processName}. [processId={process.Id}, subMode={subMode}]");

                _runningProcessIds.Add(process.Id);
                targetSubMode = subMode;
                break;
            }

            await SetIntelligentSubModeAsync(targetSubMode).ConfigureAwait(false);

            _startProcessListener = WMI.Win32.ProcessStartTrace.Listen(ProcessStarted);
            _stopProcessListener = WMI.Win32.ProcessStopTrace.Listen(ProcessStopped);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Initial sub mode set.");
        }
        catch (TaskCanceledException)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Initial sub mode set cancelled.");
        }
    }

    private static async Task SetIntelligentSubModeAsync(int subMode)
    {
        try
        {
            await WMI.LenovoGameZoneData.SetIntelligentSubModeAsync(subMode).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Sub mode set to {subMode}.");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to set sub mode {subMode}.", ex);
        }
    }

    private static async Task<int> GetIntelligentSubModeAsync()
    {
        try
        {
            var subMode = await WMI.LenovoGameZoneData.GetIntelligentSubModeAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Sub mode currently set to {subMode}.");

            return subMode;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get sub mode.", ex);

            throw;
        }
    }
}
