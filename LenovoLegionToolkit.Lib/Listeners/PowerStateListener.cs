using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners;

public class PowerStateListener : IListener<EventArgs>
{
    private readonly IGPUModeFeature _igpuModeFeature;
    private readonly RGBKeyboardBacklightController _rgbController;

    private bool _started;
    private PowerAdapterStatus? _lastState;

    public event EventHandler<EventArgs>? Changed;

    public PowerStateListener(IGPUModeFeature igpuModeFeature, RGBKeyboardBacklightController rgbController)
    {
        _igpuModeFeature = igpuModeFeature ?? throw new ArgumentNullException(nameof(igpuModeFeature));
        _rgbController = rgbController ?? throw new ArgumentNullException(nameof(rgbController));
    }

    public async Task StartAsync()
    {
        if (_started)
            return;

        _lastState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

        SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        _started = true;
    }

    public Task StopAsync()
    {
        SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        _started = false;

        return Task.CompletedTask;
    }

    private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        var newState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received. [e.Mode={e.Mode}, newState={newState}]");

        if (e.Mode is PowerModes.Resume)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await _igpuModeFeature.NotifyAsync().ConfigureAwait(false);
            });
        }

        if (e.Mode is PowerModes.Resume or PowerModes.StatusChange)
        {
            await RestoreRGBKeyboardStateAsync(e.Mode).ConfigureAwait(false);
        }

        if (newState == _lastState)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event skipped. [newState={newState}, lastState={_lastState}]");

            return;
        }

        _lastState = newState;

        if (e.Mode is PowerModes.Suspend)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event skipped. [e.Mode={e.Mode}]");

            return;
        }

        Changed?.Invoke(this, EventArgs.Empty);

        Notify(e.Mode, newState);
    }

    private async Task RestoreRGBKeyboardStateAsync(PowerModes mode)
    {
        if (mode != PowerModes.Resume)
            return;

        try
        {
            if (await _rgbController.IsSupportedAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Setting light control owner and restoring preset...");

                await _rgbController.SetLightControlOwnerAsync(true, true).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't set light control owner or current preset.", ex);
        }
    }

    private static void Notify(PowerModes mode, PowerAdapterStatus newState)
    {
        if (mode == PowerModes.Suspend)
            return;

        switch (newState)
        {
            case PowerAdapterStatus.Connected:
                MessagingCenter.Publish(new Notification(NotificationType.ACAdapterConnected, NotificationDuration.Short));
                break;
            case PowerAdapterStatus.ConnectedLowWattage:
                MessagingCenter.Publish(new Notification(NotificationType.ACAdapterConnectedLowWattage, NotificationDuration.Short));
                break;
            case PowerAdapterStatus.Disconnected:
                MessagingCenter.Publish(new Notification(NotificationType.ACAdapterDisconnected, NotificationDuration.Short));
                break;
        }
    }
}