using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.AutoListeners;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Controllers;

public class AIController
{
    private readonly ThrottleLastDispatcher _dispatcher = new(TimeSpan.FromSeconds(1), nameof(AIController));

    private readonly AsyncLock _startStopLock = new();

    private readonly PowerModeListener _powerModeListener;
    private readonly PowerStateListener _powerStateListener;
    private readonly GameAutoListener _gameAutoListener;
    private readonly PowerModeFeature _powerModeFeature;
    private readonly BalanceModeSettings _settings;

    public bool IsAIModeEnabled
    {
        get => _settings.Store.AIModeEnabled;
        set
        {
            _settings.Store.AIModeEnabled = value;
            _settings.SynchronizeStore();
        }
    }

    public AIController(PowerModeListener powerModeListener,
        PowerStateListener powerStateListener,
        GameAutoListener gameAutoListener,
        PowerModeFeature powerModeFeature,
        BalanceModeSettings settings)
    {
        _powerModeListener = powerModeListener ?? throw new ArgumentNullException(nameof(powerModeListener));
        _powerStateListener = powerStateListener ?? throw new ArgumentNullException(nameof(powerStateListener));
        _gameAutoListener = gameAutoListener ?? throw new ArgumentNullException(nameof(gameAutoListener));
        _powerModeFeature = powerModeFeature ?? throw new ArgumentNullException(nameof(powerModeFeature));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task StartIfNeededAsync()
    {
        if (!await IsSupportedAsync().ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not supported.");

            return;
        }

        await StopAsync();

        if (!IsAIModeEnabled)
            return;

        using (await _startStopLock.LockAsync().ConfigureAwait(false))
        {
            _powerModeListener.Changed += PowerModeListener_Changed;
            _powerStateListener.Changed += PowerStateListener_Changed;
            _gameAutoListener.Changed += GameAutoListenerChanged;

            await RefreshAsync().ConfigureAwait(false);
        }
    }

    public async Task StopAsync()
    {
        if (!await IsSupportedAsync().ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not supported.");

            return;
        }

        using (await _startStopLock.LockAsync().ConfigureAwait(false))
        {
            _powerModeListener.Changed -= PowerModeListener_Changed;
            _powerStateListener.Changed -= PowerStateListener_Changed;
            _gameAutoListener.Changed -= GameAutoListenerChanged;

            if (await ShouldDisableAsync().ConfigureAwait(false))
                await DisableAsync().ConfigureAwait(false);
        }
    }

    private async void PowerModeListener_Changed(object? sender, PowerModeState e) => await _dispatcher.DispatchAsync(RefreshAsync);
    private async void PowerStateListener_Changed(object? sender, EventArgs e) => await _dispatcher.DispatchAsync(RefreshAsync);
    private async void GameAutoListenerChanged(object? sender, bool e) => await _dispatcher.DispatchAsync(RefreshAsync);

    private async Task RefreshAsync()
    {
        if (!await IsSupportedAsync().ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not supported.");

            return;
        }

        using (await _startStopLock.LockAsync().ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Refreshing...");

            if (await ShouldDisableAsync().ConfigureAwait(false))
                await DisableAsync().ConfigureAwait(false);

            if (await ShouldEnableAsync().ConfigureAwait(false))
                await EnableAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Refreshed");
        }
    }

    private static async Task<bool> IsSupportedAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        return mi.Properties.SupportsAIMode;
    }

    private async Task<bool> ShouldEnableAsync()
    {
        if (await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false) != PowerAdapterStatus.Connected)
            return false;

        if (await _powerModeFeature.GetStateAsync().ConfigureAwait(false) != PowerModeState.Balance)
            return false;

        if (!_gameAutoListener.AreGamesRunning())
            return false;

        return true;
    }

    private async Task<bool> ShouldDisableAsync()
    {
        if (await _powerModeFeature.GetStateAsync().ConfigureAwait(false) != PowerModeState.Balance)
            return false;

        if (await GetIntelligentSubModeAsync().ConfigureAwait(false) == 0)
            return false;

        return true;
    }

    private static async Task EnableAsync()
    {
        var targetSubMode = 1;

        var intelligentOpList = await GetIntelligentOpListAsync().ConfigureAwait(false);
        foreach (var (processName, subMode) in intelligentOpList)
        {
            var process = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName)).FirstOrDefault();
            if (process is null)
                continue;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Found running process {processName}. [processId={process.Id}, subMode={subMode}]");

            targetSubMode = subMode;
            break;
        }

        await SetIntelligentSubModeAsync(targetSubMode).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Initial sub mode set.");
    }

    private static async Task DisableAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopping...");

        await SetIntelligentSubModeAsync(0).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopped");
    }

    private static async Task<Dictionary<string, int>> GetIntelligentOpListAsync()
    {
        try
        {
            var result = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_INTELLIGENT_OP_LIST",
                pdc =>
                {
                    // ReSharper disable once StringLiteralTypo
                    var processName = pdc["processname"].Value.ToString();
                    var mode = Convert.ToInt32(pdc["mode"].Value);
                    return (processName, mode);
                }).ConfigureAwait(false);

            var dict = result.OfType<(string, int)>().ToDictionary(sm => sm.Item1, sm => sm.Item2);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Loaded {dict.Count} sub modes.");

            return dict;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to load sub modes.", ex);

            return new Dictionary<string, int>();
        }
    }

    private static async Task<int> GetIntelligentSubModeAsync()
    {
        try
        {
            var subMode = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                "GetIntelligentSubMode",
                new(),
                pdc => Convert.ToInt32(pdc["Data"].Value)).ConfigureAwait(false);

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

    private static async Task SetIntelligentSubModeAsync(int subMode)
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
