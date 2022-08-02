using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class DriverKeyListener : IListener<DriverKey>
    {
        public event EventHandler<DriverKey>? Changed;

        private readonly Task _listenTask;
        private readonly FnKeys _fnKeys;
        private readonly TouchpadLockFeature _touchpadLockFeature;

        public DriverKeyListener(FnKeys fnKeys, TouchpadLockFeature touchpadLockFeature)
        {
            _fnKeys = fnKeys ?? throw new ArgumentNullException(nameof(fnKeys));
            _touchpadLockFeature = touchpadLockFeature ?? throw new ArgumentNullException(nameof(touchpadLockFeature));

            _listenTask = Task.Run(HandlerAsync);
        }

        private async Task HandlerAsync()
        {
            try
            {
                var resetEvent = new ManualResetEvent(false);
                var handle = (uint)resetEvent.SafeWaitHandle.DangerousGetHandle();
                var setHandleResult = Native.DeviceIoControl(Drivers.GetEnergy(),
                                                     0x831020D8,
                                                     ref handle,
                                                     0x10,
                                                     out _,
                                                     0,
                                                     out _,
                                                     IntPtr.Zero);
                if (!setHandleResult)
                    NativeUtils.ThrowIfWin32Error("DeviceIoControl, setHandleResult");

                while (true)
                {
                    resetEvent.WaitOne();

                    if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                        resetEvent.Reset();
                        continue;
                    }

                    uint inBuff = 0;
                    var getValueResult = Native.DeviceIoControl(Drivers.GetEnergy(),
                                                                0x831020CC,
                                                                ref inBuff,
                                                                0x4,
                                                                out uint value,
                                                                0x4,
                                                                out _,
                                                                IntPtr.Zero);
                    if (!getValueResult)
                        NativeUtils.ThrowIfWin32Error("DeviceIoControl, getValueResult");

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
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Unknown error.", ex);
            }
        }

        protected async Task OnChangedAsync(DriverKey value)
        {
            try
            {
                switch (value)
                {
                    case DriverKey.Fn_F4:
                        var enabled = Microphone.Toggle();

                        if (enabled)
                            MessagingCenter.Publish(new Notification(NotificationIcon.MicrophoneOn, "Microphone on", NotificationDuration.Short));
                        else
                            MessagingCenter.Publish(new Notification(NotificationIcon.MicrophoneOff, "Microphone off", NotificationDuration.Short));

                        break;
                    case DriverKey.Fn_F10:
                        var status = await _touchpadLockFeature.GetStateAsync().ConfigureAwait(false);

                        if (status == TouchpadLockState.Off)
                            MessagingCenter.Publish(new Notification(NotificationIcon.TouchpadOn, "Touchpad on", NotificationDuration.Short));
                        else
                            MessagingCenter.Publish(new Notification(NotificationIcon.TouchpadOff, "Touchpad off", NotificationDuration.Short));

                        break;
                    case DriverKey.Fn_F8:
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd",
                            Arguments = "/c \"start ms-settings:network-airplanemode\"",
                            UseShellExecute = true,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Unknown exception. [value={value}]", ex);
            }
        }
    }
}
