using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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

        public DriverKeyListener(FnKeys fnKeys)
        {
            _fnKeys = fnKeys;

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

                    if (await _fnKeys.GetStatusAsync() == SoftwareStatus.Enabled)
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

                        await OnChangedAsync(key);
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
                    Log.Instance.Trace($"Unknown error: {ex.Demystify()}");
            }
        }

        protected Task OnChangedAsync(DriverKey value)
        {
            try
            {
                //if (value == DriverKey.Fn_F4)
                if (value == DriverKey.Fn_F8)
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
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Could not run action:{ex.Demystify()} [value={value}]");
            }

            return Task.CompletedTask;
        }
    }
}
