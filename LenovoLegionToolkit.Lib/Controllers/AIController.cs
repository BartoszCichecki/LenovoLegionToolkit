using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.AutoListeners;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Controllers;

public class AIController(
    PowerModeListener powerModeListener,
    PowerStateListener powerStateListener,
    GameAutoListener gameAutoListener,
    PowerModeFeature powerModeFeature,
    BalanceModeSettings settings)
{
    private readonly ThrottleLastDispatcher _dispatcher = new(TimeSpan.FromSeconds(1), nameof(AIController));

    private readonly AsyncLock _startStopLock = new();

    public bool IsAIModeEnabled
    {
        get => settings.Store.AIModeEnabled;
        set
        {
            settings.Store.AIModeEnabled = value;
            settings.SynchronizeStore();
        }
    }

    public async Task StartIfNeededAsync()
    {
        if (!await IsSupportedAsync().ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not supported.");

            return;
        }

        await StopAsync().ConfigureAwait(false);

        if (!IsAIModeEnabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"AI Mode is not enabled.");
            return;
        }

        using (await _startStopLock.LockAsync().ConfigureAwait(false))
        {
            powerModeListener.Changed += PowerModeListener_Changed;
            powerStateListener.Changed += PowerStateListener_Changed;

            await gameAutoListener.SubscribeChangedAsync(GameAutoListener_Changed).ConfigureAwait(false);

            await RefreshAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Started.");
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
            powerModeListener.Changed -= PowerModeListener_Changed;
            powerStateListener.Changed -= PowerStateListener_Changed;

            await gameAutoListener.UnsubscribeChangedAsync(GameAutoListener_Changed).ConfigureAwait(false);

            if (await ShouldDisableAsync().ConfigureAwait(false))
                await DisableAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopped.");
        }
    }

    private async void PowerModeListener_Changed(object? sender, PowerModeListener.ChangedEventArgs e) => await _dispatcher.DispatchAsync(RefreshAsync).ConfigureAwait(false);
    private async void PowerStateListener_Changed(object? sender, PowerStateListener.ChangedEventArgs e) => await _dispatcher.DispatchAsync(RefreshAsync).ConfigureAwait(false);
    private async void GameAutoListener_Changed(object? sender, GameAutoListener.ChangedEventArgs e) => await _dispatcher.DispatchAsync(RefreshAsync).ConfigureAwait(false);

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
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power adapter not connected.");

            return false;
        }

        if (await powerModeFeature.GetStateAsync().ConfigureAwait(false) != PowerModeState.Balance)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not in balanced mode.");

            return false;
        }

        if (!gameAutoListener.AreGamesRunning())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Games aren't running.");

            return false;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"All conditions met.");

        return true;
    }

    private async Task<bool> ShouldDisableAsync()
    {
        if (await powerModeFeature.GetStateAsync().ConfigureAwait(false) != PowerModeState.Balance)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not in balanced mode.");

            return false;
        }

        if (await WMI.LenovoGameZoneData.GetIntelligentSubModeAsync().ConfigureAwait(false) == 0)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not needed.");

            return false;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"All conditions met.");

        return true;
    }

    private static async Task EnableAsync()
    {
        try
        {
            var targetSubMode = 1;

            var intelligentOpList = await WMI.LenovoIntelligentOPList.ReadAsync().ConfigureAwait(false);
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

            await WMI.LenovoGameZoneData.SetIntelligentSubModeAsync(targetSubMode).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Initial sub mode set.");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to start.", ex);
        }
    }

    private static async Task DisableAsync()
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopping...");

            await WMI.LenovoGameZoneData.SetIntelligentSubModeAsync(0).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopped");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to stop.", ex);
        }
    }
}
