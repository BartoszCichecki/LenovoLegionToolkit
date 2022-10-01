using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal class SystemEventInterceptor : NativeWindow
    {
        // Should free this on exit
        private IntPtr _displayArrivalHandle;

        public SystemEventInterceptor(IntPtr handle)
        {
            _displayArrivalHandle = RegisterDisplayArrival(handle);

            AssignHandle(handle);
        }

        private IntPtr RegisterDisplayArrival(IntPtr handle)
        {
            var ptr = IntPtr.Zero;
            try
            {
                var str = new Native.DevBroadcastDeviceInterface();
                str.Size = Marshal.SizeOf(str);
                str.DeviceType = Native.DBT_DEVTYPE_HANDLE;
                str.ClassGuid = Native.GUID_DISPLAY_DEVICE_ARRIVAL;
                ptr = Marshal.AllocHGlobal(Marshal.SizeOf(str));
                Marshal.StructureToPtr(str, ptr, true);
                return Native.User32.RegisterDeviceNotification(handle, ptr, 0U);
            }
            finally
            {

                Marshal.FreeHGlobal(ptr);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Native.WM_DEVICECHANGE)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"WM_DEVICECHANGE");

                if (m.LParam != IntPtr.Zero)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"m.LParam != IntPtr.Zero");

                    var devBroadcastHdr = Marshal.PtrToStructure<Native.DevBroadcastHdr>(m.LParam);
                    if (devBroadcastHdr.DeviceType == Native.DBT_DEVTYPE_HANDLE)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"devBroadcastHdr.DeviceType == DBT_DEVTYP_HANDLE");

                        var devBroadcastDeviceInterface = Marshal.PtrToStructure<Native.DevBroadcastDeviceInterface>(m.LParam);
                        if (devBroadcastDeviceInterface.ClassGuid == Native.GUID_DISPLAY_DEVICE_ARRIVAL)
                        {
                            if (Log.Instance.IsTraceEnabled)
                                Log.Instance.Trace($"devBroadcastDeviceInterface.ClassGuid == GUID_DISPLAY_DEVICE_ARRIVAL");

                            if (Log.Instance.IsTraceEnabled)
                                Log.Instance.Trace($"NotifyDGPU");

                            NotifyDGPU();
                        }
                    }
                }
            }

            base.WndProc(ref m);
        }

        // Some error handling would be nice and no async void
        private async void NotifyDGPU()
        {
            var feat = IoCContainer.Resolve<IGPUModeFeature>();

            if (!await feat.IsSupportedAsync())
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Notifying...");

            var state = await feat.GetStateAsync();
            await feat.NotifyDGPUStatusAsync(state);
        }
    }
}
