using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal class SystemEventInterceptor : NativeWindow
    {
        private readonly uint _taskbarCreatedMessageId;
        private readonly IntPtr _displayArrivalHandle;

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
            if (_displayArrivalHandle != IntPtr.Zero)
                _ = Native.User32.UnregisterDeviceNotification(_displayArrivalHandle);
        }

        private uint RegisterTaskbarCreatedMessage()
        {
            var message = Native.User32.RegisterWindowMessage("TaskbarCreated");
            Native.User32.ChangeWindowMessageFilter(message, 1u);
            return message;
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
                return Native.User32.RegisterDeviceNotification(handle, ptr, 0u);
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
                OnTaskbarCreated?.Invoke(this, EventArgs.Empty);
            }

            if (m.Msg == Native.WM_DEVICECHANGE && m.LParam != IntPtr.Zero)
            {
                var devBroadcastHdr = Marshal.PtrToStructure<Native.DevBroadcastHdr>(m.LParam);
                if (devBroadcastHdr.DeviceType == Native.DBT_DEVTYPE_HANDLE)
                {
                    var devBroadcastDeviceInterface = Marshal.PtrToStructure<Native.DevBroadcastDeviceInterface>(m.LParam);
                    if (devBroadcastDeviceInterface.ClassGuid == Native.GUID_DISPLAY_DEVICE_ARRIVAL)
                        OnDisplayDeviceArrival?.Invoke(this, EventArgs.Empty);
                }
            }

            base.WndProc(ref m);
        }
    }
}
