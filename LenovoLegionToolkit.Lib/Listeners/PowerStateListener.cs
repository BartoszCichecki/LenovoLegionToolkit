using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;

namespace LenovoLegionToolkit.Lib.Listeners;

public class PowerStateListener : IListener<EventArgs>
{
    private enum PowerMode
    {
        Unknown = -1,
        StatusChange,
        Suspend,
        Resume,
    }

    private readonly SafeHandle _recipientHandle;
    private readonly PDEVICE_NOTIFY_CALLBACK_ROUTINE _callback;

    private readonly PowerModeFeature _powerModeFeature;
    private readonly IGPUModeFeature _iGpuModeFeature;
    private readonly BatteryFeature _batteryFeature;
    private readonly RGBKeyboardBacklightController _rgbController;

    private bool _started;
    private HPOWERNOTIFY _handle;
    private PowerAdapterStatus? _lastState;

    public event EventHandler<EventArgs>? Changed;

    public unsafe PowerStateListener(PowerModeFeature powerModeFeature, IGPUModeFeature iGpuModeFeature, BatteryFeature batteryFeature, RGBKeyboardBacklightController rgbController)
    {
        _powerModeFeature = powerModeFeature ?? throw new ArgumentNullException(nameof(powerModeFeature));
        _iGpuModeFeature = iGpuModeFeature ?? throw new ArgumentNullException(nameof(iGpuModeFeature));
        _batteryFeature = batteryFeature ?? throw new ArgumentNullException(nameof(batteryFeature));
        _rgbController = rgbController ?? throw new ArgumentNullException(nameof(rgbController));

        _callback = Callback;
        _recipientHandle = new StructSafeHandle<DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS>(new DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS
        {
            Callback = _callback,
            Context = null,
        });
    }

    public async Task StartAsync()
    {
        if (_started)
            return;

        _lastState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

        SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        RegisterSuspendResumeNotification();

        _started = true;
    }

    public Task StopAsync()
    {
        SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        UnRegisterSuspendResumeNotification();

        _started = false;

        return Task.CompletedTask;
    }

    private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received: {e.Mode}");

        var powerMode = e.Mode switch
        {
            PowerModes.StatusChange => PowerMode.StatusChange,
            PowerModes.Resume => PowerMode.Resume,
            PowerModes.Suspend => PowerMode.Suspend,
            _ => PowerMode.Unknown
        };

        if (powerMode is PowerMode.Unknown)
            return;

        await HandleAsync(powerMode).ConfigureAwait(false);
    }

    private unsafe uint Callback(void* context, uint type, void* setting)
    {
        _ = Task.Run(() => CallbackAsync(type));
        return (uint)WIN32_ERROR.ERROR_SUCCESS;
    }

    private async Task CallbackAsync(uint type)
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        if (!mi.Properties.SupportsAlwaysOnAc.status)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ignoring, AO AC not enabled...");

            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event value: {type}");

        var powerMode = type switch
        {
            PInvoke.PBT_APMRESUMEAUTOMATIC => PowerMode.Resume,
            _ => PowerMode.Unknown
        };

        if (powerMode is not PowerMode.Resume)
            return;

        await HandleAsync(powerMode).ConfigureAwait(false);
    }

    private async Task HandleAsync(PowerMode mode)
    {
        var newState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Handle {mode}. [newState={newState}]");

        if (mode is PowerMode.Resume)
        {
            _ = Task.Run(async () =>
            {
                if (await _batteryFeature.IsSupportedAsync().ConfigureAwait(false))
                    await _batteryFeature.EnsureCorrectBatteryModeIsSetAsync().ConfigureAwait(false);

                if (await _rgbController.IsSupportedAsync().ConfigureAwait(false))
                    await _rgbController.SetLightControlOwnerAsync(true, true).ConfigureAwait(false);

                if (await _powerModeFeature.IsSupportedAsync().ConfigureAwait(false))
                {
                    await _powerModeFeature.EnsureAiModeIsSetAsync().ConfigureAwait(false);
                    await _powerModeFeature.EnsureGodModeStateIsAppliedAsync().ConfigureAwait(false);
                    await _powerModeFeature.EnsureCorrectPowerPlanIsSetAsync().ConfigureAwait(false);
                }

                if (await _iGpuModeFeature.IsSupportedAsync().ConfigureAwait(false))
                {
                    await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    await _iGpuModeFeature.NotifyAsync().ConfigureAwait(false);
                }
            });
        }

        if (mode is PowerMode.StatusChange && newState is PowerAdapterStatus.Connected)
        {
            _ = Task.Run(async () =>
            {
                if (await _powerModeFeature.IsSupportedAsync().ConfigureAwait(false))
                    await _powerModeFeature.EnsureGodModeStateIsAppliedAsync().ConfigureAwait(false);
            });
        }

        if (newState == _lastState)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event skipped. [newState={newState}, lastState={_lastState}]");

            return;
        }

        _lastState = newState;

        if (mode is PowerMode.Suspend)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event skipped. [mode={mode}]");

            return;
        }

        Changed?.Invoke(this, EventArgs.Empty);

        Notify(newState);
    }

    private unsafe void RegisterSuspendResumeNotification()
    {
        _handle = PInvoke.PowerRegisterSuspendResumeNotification(PInvokeExtensions.DEVICE_NOTIFY_CALLBACK, _recipientHandle, out var handle) == WIN32_ERROR.ERROR_SUCCESS
            ? new HPOWERNOTIFY(new IntPtr(handle))
            : HPOWERNOTIFY.Null;
    }

    private void UnRegisterSuspendResumeNotification()
    {
        PInvoke.PowerUnregisterSuspendResumeNotification(_handle);
        _handle = HPOWERNOTIFY.Null;
    }

    private static void Notify(PowerAdapterStatus newState)
    {
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
