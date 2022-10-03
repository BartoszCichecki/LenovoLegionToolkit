using System;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal static class Native
    {
        internal static int WM_DEVICECHANGE = 0x0219;
        internal static int DBT_DEVTYPE_HANDLE = 5;
        internal static Guid GUID_DISPLAY_DEVICE_ARRIVAL = new("1CA05180-A699-450A-9A0C-DE4FBE3DDD89");

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
            public int DeviceType;
            public int Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string Name;
        }

        internal static class User32
        {
            [DllImport("user32.dll")]
            public static extern bool ChangeWindowMessageFilter(uint message, uint dwFlag);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr RegisterDeviceNotification(IntPtr ptr, IntPtr notificationFilter, uint flags);

            [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW")]
            public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern uint UnregisterDeviceNotification(IntPtr ptr);
        }
    }
}
