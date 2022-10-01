using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal class SystemEventInterceptor : NativeWindow
    {
        private static int WM_DEVICECHANGE = 0x0219;

        private static int DBT_DEVTYP_HANDLE = 5;
        private static Guid GUID_DISPLAY_DEVICE_ARRIVAL = new("1CA05180-A699-450A-9A0C-DE4FBE3DDD89");

        public SystemEventInterceptor(IntPtr handle)
        {
            AssignHandle(handle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"WM_DEVICECHANGE");

                if (m.LParam != IntPtr.Zero)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"m.LParam != IntPtr.Zero");

                    var devBroadcastHdr = Marshal.PtrToStructure<DevBroadcastHdr>(m.LParam);
                    if (devBroadcastHdr.DeviceType == DBT_DEVTYP_HANDLE)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"devBroadcastHdr.DeviceType == DBT_DEVTYP_HANDLE");

                        var devBroadcastDeviceInterface = Marshal.PtrToStructure<DevBroadcastDeviceInterface>(m.LParam);
                        if (devBroadcastDeviceInterface.ClassGuid == GUID_DISPLAY_DEVICE_ARRIVAL)
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct DevBroadcastHdr
    {
        public int Size;
        public int DeviceType;
        public int Reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct DevBroadcastDeviceInterface
    {
        public int Size;
        public int Devicetype;
        public int Reserved;
        public Guid ClassGuid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string Name;
    }
}
