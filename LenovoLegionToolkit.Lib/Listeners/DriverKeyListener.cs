﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class DriverKeyListener : IListener<DriverKey>
    {
        public event EventHandler<DriverKey>? Changed;

        private readonly FnKeys _fnKeys;
        private readonly TouchpadLockFeature _touchpadLockFeature;
        private readonly WhiteKeyboardBacklightFeature _whiteKeyboardBacklightFeature;

        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _listenTask;
        private bool _ignoreNext;

        public DriverKeyListener(FnKeys fnKeys, TouchpadLockFeature touchpadLockFeature, WhiteKeyboardBacklightFeature whiteKeyboardBacklightFeature)
        {
            _fnKeys = fnKeys ?? throw new ArgumentNullException(nameof(fnKeys));
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

        public Task StopAsync()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            _listenTask = null;

            return Task.CompletedTask;
        }

        public void IgnoreNext() => _ignoreNext = true;

        private async Task HandlerAsync(CancellationToken token)
        {
            try
            {
                var resetEvent = new ManualResetEvent(false);
                var setHandleResult = BindListener(resetEvent);
                if (!setHandleResult)
                    PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, setHandleResult");

                while (true)
                {
                    WaitHandle.WaitAny(new[] { resetEvent, token.WaitHandle });

                    token.ThrowIfCancellationRequested();

                    if (_ignoreNext)
                    {
                        _ignoreNext = false;
                        resetEvent.Reset();
                        continue;
                    }

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
                    if (Enum.IsDefined(key))
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Event received. [key={key}]");

                        await OnChangedAsync(key).ConfigureAwait(false);
                        Changed?.Invoke(this, key);
                    }
                    else
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Unknown value received. [value={value}]");
                    }

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
                switch (value)
                {
                    case DriverKey.Fn_F4:
                        var enabled = Microphone.Toggle();
                        if (enabled)
                            MessagingCenter.Publish(new Notification(NotificationType.MicrophoneOn, NotificationDuration.Short));
                        else
                            MessagingCenter.Publish(new Notification(NotificationType.MicrophoneOff, NotificationDuration.Short));
                        break;

                    case DriverKey.Fn_F10 or DriverKey.Fn_F10_2:
                        if (!await _touchpadLockFeature.IsSupportedAsync().ConfigureAwait(false))
                            break;

                        var status = await _touchpadLockFeature.GetStateAsync().ConfigureAwait(false);
                        if (status == TouchpadLockState.Off)
                            MessagingCenter.Publish(new Notification(NotificationType.TouchpadOn, NotificationDuration.Short));
                        else
                            MessagingCenter.Publish(new Notification(NotificationType.TouchpadOff, NotificationDuration.Short));
                        break;

                    case DriverKey.Fn_F8 or DriverKey.Fn_F8_2:
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd",
                            Arguments = "/c \"start ms-settings:network-airplanemode\"",
                            UseShellExecute = true,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        });
                        break;

                    case DriverKey.Fn_Space:
                        if (!await _whiteKeyboardBacklightFeature.IsSupportedAsync().ConfigureAwait(false))
                            break;

                        var state = await _whiteKeyboardBacklightFeature.GetStateAsync().ConfigureAwait(false);
                        MessagingCenter.Publish(new Notification(NotificationType.WhiteKeyboardBacklight, NotificationDuration.Short, state.GetDisplayName()));
                        break;
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
}
