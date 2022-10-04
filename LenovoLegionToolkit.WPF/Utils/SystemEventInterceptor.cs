using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using LenovoLegionToolkit.Lib.Utils;
using Vanara.PInvoke;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal class SystemEventInterceptor : NativeWindow
    {
        internal static Guid GUID_DISPLAY_DEVICE_ARRIVAL = new("1CA05180-A699-450A-9A0C-DE4FBE3DDD89");

        private readonly uint _taskbarCreatedMessageId;
        private readonly User32.SafeHDEVNOTIFY _displayArrivalHandle;

        public event EventHandler? OnTaskbarCreated;
        public event EventHandler? OnDisplayDeviceArrival;

        public SystemEventInterceptor(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;

            _taskbarCreatedMessageId = RegisterTaskbarCreatedMessage();
            _displayArrivalHandle = RegisterDisplayArrival(handle);

            AssignHandle(handle);
        }

        ~SystemEventInterceptor()
        {
            if (_displayArrivalHandle != User32.HDEVNOTIFY.NULL)
                User32.UnregisterDeviceNotification(_displayArrivalHandle);
        }

        private static uint RegisterTaskbarCreatedMessage()
        {
            var message = User32.RegisterWindowMessage("TaskbarCreated");
            User32.ChangeWindowMessageFilter(message, User32.MessageFilterFlag.MSGFLT_ADD);
            return message;
        }

        private static User32.SafeHDEVNOTIFY RegisterDisplayArrival(IntPtr handle)
        {
            var ptr = IntPtr.Zero;
            try
            {
                var str = new User32.DEV_BROADCAST_DEVICEINTERFACE();
                str.dbcc_size = (uint)Marshal.SizeOf(str);
                str.dbcc_devicetype = User32.DBT_DEVTYPE.DBT_DEVTYP_DEVICEINTERFACE;
                str.dbcc_classguid = GUID_DISPLAY_DEVICE_ARRIVAL;
                ptr = Marshal.AllocHGlobal(Marshal.SizeOf(str));
                Marshal.StructureToPtr(str, ptr, true);
                return User32.RegisterDeviceNotification(handle, ptr, User32.DEVICE_NOTIFY.DEVICE_NOTIFY_WINDOW_HANDLE);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == _taskbarCreatedMessageId)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"TaskbarCreated received.");

                OnTaskbarCreated?.Invoke(this, EventArgs.Empty);
            }

            if (m.Msg == User32.WM_DEVICECHANGE)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"WM_DEVICECHANGE received.");

                if (m.LParam != IntPtr.Zero)
                {
                    var devBroadcastHdr = Marshal.PtrToStructure<User32.DEV_BROADCAST_HDR>(m.LParam);
                    if (devBroadcastHdr.dbch_devicetype == User32.DBT_DEVTYPE.DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        var devBroadcastDeviceInterface = Marshal.PtrToStructure<User32.DEV_BROADCAST_DEVICEINTERFACE>(m.LParam);
                        if (devBroadcastDeviceInterface.dbcc_classguid == GUID_DISPLAY_DEVICE_ARRIVAL)
                            OnDisplayDeviceArrival?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            base.WndProc(ref m);
        }
    }
}
