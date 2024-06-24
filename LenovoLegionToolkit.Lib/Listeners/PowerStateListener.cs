using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Features.Hybrid.Notify;
using LenovoLegionToolkit.Lib.Messaging;
using LenovoLegionToolkit.Lib.Messaging.Messages;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Listeners;

public class PowerStateListener : IListener<PowerStateListener.ChangedEventArgs>
{
    public class ChangedEventArgs(PowerStateEvent powerStateEvent, bool powerAdapterStateChanged) : EventArgs
    {
        public PowerStateEvent PowerStateEvent { get; } = powerStateEvent;
        public bool PowerAdapterStateChanged { get; } = powerAdapterStateChanged;
    }

    private readonly SafeHandle _recipientHandle;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly PDEVICE_NOTIFY_CALLBACK_ROUTINE _callback;

    private readonly PowerModeFeature _powerModeFeature;
    private readonly BatteryFeature _batteryFeature;
    private readonly DGPUNotify _dgpuNotify;
    private readonly RGBKeyboardBacklightController _rgbController;

    private bool _started;
    private HPOWERNOTIFY _handle;
    private PowerAdapterStatus? _lastPowerAdapterState;

    public event EventHandler<ChangedEventArgs>? Changed;

    public unsafe PowerStateListener(PowerModeFeature powerModeFeature, BatteryFeature batteryFeature, DGPUNotify dgpuNotify, RGBKeyboardBacklightController rgbController)
    {
        _powerModeFeature = powerModeFeature;
        _batteryFeature = batteryFeature;
        _dgpuNotify = dgpuNotify;
        _rgbController = rgbController;

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

        _lastPowerAdapterState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

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
            PowerModes.StatusChange => PowerStateEvent.StatusChange,
            PowerModes.Resume => PowerStateEvent.Resume,
            PowerModes.Suspend => PowerStateEvent.Suspend,
            _ => PowerStateEvent.Unknown
        };

        if (powerMode is PowerStateEvent.Unknown)
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
            PInvoke.PBT_APMRESUMEAUTOMATIC => PowerStateEvent.Resume,
            _ => PowerStateEvent.Unknown
        };

        if (powerMode is not PowerStateEvent.Resume)
            return;

        await HandleAsync(powerMode).ConfigureAwait(false);
    }

    private async Task HandleAsync(PowerStateEvent powerStateEvent)
    {
        var powerAdapterState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Handle {powerStateEvent}. [newState={powerAdapterState}]");

        if (powerStateEvent is PowerStateEvent.Resume)
        {
            _ = Task.Run(async () =>
            {
                if (await _batteryFeature.IsSupportedAsync().ConfigureAwait(false))
                    await _batteryFeature.EnsureCorrectBatteryModeIsSetAsync().ConfigureAwait(false);

                if (await _rgbController.IsSupportedAsync().ConfigureAwait(false))
                    await _rgbController.SetLightControlOwnerAsync(true, true).ConfigureAwait(false);

                if (await _powerModeFeature.IsSupportedAsync().ConfigureAwait(false))
                {
                    await _powerModeFeature.EnsureCorrectWindowsPowerSettingsAreSetAsync().ConfigureAwait(false);
                    await _powerModeFeature.EnsureGodModeStateIsAppliedAsync().ConfigureAwait(false);
                }

                if (await _dgpuNotify.IsSupportedAsync().ConfigureAwait(false))
                {
                    await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    await _dgpuNotify.NotifyAsync().ConfigureAwait(false);
                }
            });
        }

        if (powerStateEvent is PowerStateEvent.StatusChange && powerAdapterState is PowerAdapterStatus.Connected)
        {
            _ = Task.Run(async () =>
            {
                if (await _powerModeFeature.IsSupportedAsync().ConfigureAwait(false))
                    await _powerModeFeature.EnsureGodModeStateIsAppliedAsync().ConfigureAwait(false);

                if (await _dgpuNotify.IsSupportedAsync().ConfigureAwait(false))
                {
                    await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    await _dgpuNotify.NotifyAsync().ConfigureAwait(false);
                }
            });
        }

        var powerAdapterStateChanged = powerAdapterState != _lastPowerAdapterState;
        _lastPowerAdapterState = powerAdapterState;

        if (powerStateEvent is PowerStateEvent.Suspend or PowerStateEvent.Unknown)
            return;

        if (powerAdapterStateChanged)
            Notify(powerAdapterState);

        Changed?.Invoke(this, new(powerStateEvent, powerAdapterStateChanged));
    }

    private unsafe void RegisterSuspendResumeNotification()
    {
        _handle = PInvoke.PowerRegisterSuspendResumeNotification(REGISTER_NOTIFICATION_FLAGS.DEVICE_NOTIFY_CALLBACK, _recipientHandle, out var handle) == WIN32_ERROR.ERROR_SUCCESS
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
                MessagingCenter.Publish(new NotificationMessage(NotificationType.ACAdapterConnected));
                break;
            case PowerAdapterStatus.ConnectedLowWattage:
                MessagingCenter.Publish(new NotificationMessage(NotificationType.ACAdapterConnectedLowWattage));
                break;
            case PowerAdapterStatus.Disconnected:
                MessagingCenter.Publish(new NotificationMessage(NotificationType.ACAdapterDisconnected));
                break;
        }
    }
}
