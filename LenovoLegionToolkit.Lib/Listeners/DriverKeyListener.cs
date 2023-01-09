using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Listeners;

public class DriverKeyListener : IListener<DriverKey>
{
    public event EventHandler<DriverKey>? Changed;

    private readonly FnKeys _fnKeys;
    private readonly MicrophoneFeature _microphoneFeature;
    private readonly TouchpadLockFeature _touchpadLockFeature;
    private readonly WhiteKeyboardBacklightFeature _whiteKeyboardBacklightFeature;

    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _listenTask;

    public DriverKeyListener(FnKeys fnKeys, MicrophoneFeature microphoneFeature, TouchpadLockFeature touchpadLockFeature, WhiteKeyboardBacklightFeature whiteKeyboardBacklightFeature)
    {
        _fnKeys = fnKeys ?? throw new ArgumentNullException(nameof(fnKeys));
        _microphoneFeature = microphoneFeature ?? throw new ArgumentNullException(nameof(microphoneFeature));
        _touchpadLockFeature = touchpadLockFeature ?? throw new ArgumentNullException(nameof(touchpadLockFeature));
        _whiteKeyboardBacklightFeature = whiteKeyboardBacklightFeature ?? throw new ArgumentNullException(nameof(whiteKeyboardBacklightFeature));
    }

    public Task StartAsync()
    {
        if (_listenTask is not null)
            return Task.CompletedTask;

        _cancellationTokenSource = new();
        _listenTask = Task.Run(() => HandlerAsync(_cancellationTokenSource.Token));

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        if (_listenTask != null)
            await _listenTask;
        _listenTask = null;
    }

    private async Task HandlerAsync(CancellationToken token)
    {
        try
        {
            var resetEvent = new ManualResetEvent(false);
            var setHandleResult = BindListener(resetEvent);
            if (!setHandleResult)
                PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, setHandleResult");

            GetValue(out _); // Clear register

            while (true)
            {
                WaitHandle.WaitAny(new[] { resetEvent, token.WaitHandle });

                token.ThrowIfCancellationRequested();

                if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                    resetEvent.Reset();
                    continue;
                }

                var getValueResult = GetValue(out var value);
                if (!getValueResult)
                    PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, getValueResult");

                var key = (DriverKey)value;
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Event received. [key={key}, value={value}]");

                await OnChangedAsync(key).ConfigureAwait(false);
                Changed?.Invoke(this, key);

                resetEvent.Reset();
            }
        }
        catch (OperationCanceledException) { }
        catch (ThreadAbortException) { }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unknown error.", ex);
        }
    }

    private async Task OnChangedAsync(DriverKey value)
    {
        try
        {
            if (value.HasFlag(DriverKey.Fn_F4))
            {
                if (await _microphoneFeature.IsSupportedAsync().ConfigureAwait(false))
                {
                    switch (await _microphoneFeature.GetStateAsync().ConfigureAwait(false))
                    {
                        case MicrophoneState.On:
                            await _microphoneFeature.SetStateAsync(MicrophoneState.Off).ConfigureAwait(false);
                            MessagingCenter.Publish(new Notification(NotificationType.MicrophoneOff, NotificationDuration.Short));
                            break;
                        case MicrophoneState.Off:
                            await _microphoneFeature.SetStateAsync(MicrophoneState.On).ConfigureAwait(false);
                            MessagingCenter.Publish(new Notification(NotificationType.MicrophoneOn, NotificationDuration.Short));
                            break;
                    }
                }
            }

            if (value.HasFlag(DriverKey.Fn_F8))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = "/c \"start ms-settings:network-airplanemode\"",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                });
            }

            if (value.HasFlag(DriverKey.Fn_F10))
            {
                if (await _touchpadLockFeature.IsSupportedAsync().ConfigureAwait(false))
                {
                    var status = await _touchpadLockFeature.GetStateAsync().ConfigureAwait(false);
                    MessagingCenter.Publish(status == TouchpadLockState.Off
                        ? new Notification(NotificationType.TouchpadOn, NotificationDuration.Short)
                        : new Notification(NotificationType.TouchpadOff, NotificationDuration.Short));
                }
            }

            if (value.HasFlag(DriverKey.Fn_Space))
            {
                if (await _whiteKeyboardBacklightFeature.IsSupportedAsync().ConfigureAwait(false))
                {
                    var state = await _whiteKeyboardBacklightFeature.GetStateAsync().ConfigureAwait(false);
                    MessagingCenter.Publish(state == WhiteKeyboardBacklightState.Off
                        ? new Notification(NotificationType.WhiteKeyboardBacklightOff, NotificationDuration.Short, state.GetDisplayName())
                        : new Notification(NotificationType.WhiteKeyboardBacklightChanged, NotificationDuration.Short, state.GetDisplayName()));
                }
            }
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't handle key press. [value={value}]", ex);
        }
    }

    private static unsafe bool BindListener(WaitHandle waitHandle)
    {
        var handle = (uint)waitHandle.SafeWaitHandle.DangerousGetHandle();
        return PInvoke.DeviceIoControl(Drivers.GetEnergy(),
            Drivers.IOCTL_KEY_WAIT_HANDLE,
            &handle,
            16,
            null,
            0,
            null,
            null);
    }

    private static unsafe bool GetValue(out uint value)
    {
        uint inBuff = 0;
        uint outBuff = 0;
        var result = PInvoke.DeviceIoControl(Drivers.GetEnergy(),
            Drivers.IOCTL_KEY_VALUE,
            &inBuff,
            4,
            &outBuff,
            4,
            null,
            null);
        value = outBuff;
        return result;
    }
}