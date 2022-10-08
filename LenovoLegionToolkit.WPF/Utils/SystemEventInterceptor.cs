using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.System.Power;
using Windows.Win32.System.SystemServices;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal unsafe class SystemEventInterceptor : NativeWindow
    {
        private readonly SafeHandle _safeHandle;

        private readonly uint _taskbarCreatedMessageId;
        private readonly void* _displayArrivalHandle;
        private readonly SafeHandle _powerNotificationHandle;

        public event EventHandler? OnTaskbarCreated;
        public event EventHandler? OnDisplayDeviceArrival;
        public event EventHandler? OnResumed;

        public SystemEventInterceptor(Window window)
        {
            var ptr = new WindowInteropHelper(window).Handle;
            _safeHandle = new SafeAccessTokenHandle(ptr);

            _taskbarCreatedMessageId = RegisterTaskbarCreatedMessage();
            _displayArrivalHandle = RegisterDisplayArrival(_safeHandle);
            _powerNotificationHandle = RegisterPowerNotification(_safeHandle);

            AssignHandle(ptr);
        }

        ~SystemEventInterceptor()
        {
            PInvoke.UnregisterDeviceNotification(_displayArrivalHandle);
            PInvoke.UnregisterPowerSettingNotification(new HPOWERNOTIFY(_powerNotificationHandle.DangerousGetHandle()));

            _powerNotificationHandle.DangerousRelease();
            _safeHandle.Dispose();
        }

        private static uint RegisterTaskbarCreatedMessage()
        {
            var message = PInvoke.RegisterWindowMessage("TaskbarCreated");
            PInvoke.ChangeWindowMessageFilter(message, CHANGE_WINDOW_MESSAGE_FILTER_FLAGS.MSGFLT_ADD);
            return message;
        }

        private static SafeHandle RegisterPowerNotification(SafeHandle handle)
        {
            return PInvoke.RegisterPowerSettingNotification(handle, PInvoke.GUID_MONITOR_POWER_ON, 0);
        }

        private static void* RegisterDisplayArrival(SafeHandle handle)
        {
            var ptr = IntPtr.Zero;
            try
            {
                var str = new DEV_BROADCAST_DEVICEINTERFACE_W();
                str.dbcc_size = (uint)Marshal.SizeOf(str);
                str.dbcc_devicetype = (uint)DEV_BROADCAST_HDR_DEVICE_TYPE.DBT_DEVTYP_DEVICEINTERFACE;
                str.dbcc_classguid = PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL;
                ptr = Marshal.AllocHGlobal(Marshal.SizeOf(str));
                Marshal.StructureToPtr(str, ptr, true);
                return PInvoke.RegisterDeviceNotification(handle, ptr.ToPointer(), POWER_SETTING_REGISTER_NOTIFICATION_FLAGS.DEVICE_NOTIFY_WINDOW_HANDLE);
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

            if (m.Msg == PInvoke.WM_POWERBROADCAST)
            {
                if (m.WParam == (IntPtr)PInvoke.PBT_POWERSETTINGCHANGE)
                {
                    var powerBroadcastSettings = Marshal.PtrToStructure<POWERBROADCAST_SETTING>(m.LParam);
                    if (powerBroadcastSettings.PowerSetting == PInvoke.GUID_MONITOR_POWER_ON)
                    {
                        var data = powerBroadcastSettings.Data._0;
                        if (data == 1)
                            OnResumed?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            if (m.Msg == PInvoke.WM_DEVICECHANGE)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"WM_DEVICECHANGE received.");

                if (m.LParam != IntPtr.Zero)
                {
                    var devBroadcastHdr = Marshal.PtrToStructure<DEV_BROADCAST_HDR>(m.LParam);
                    if (devBroadcastHdr.dbch_devicetype == DEV_BROADCAST_HDR_DEVICE_TYPE.DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        var devBroadcastDeviceInterface = Marshal.PtrToStructure<DEV_BROADCAST_DEVICEINTERFACE_W>(m.LParam);
                        if (devBroadcastDeviceInterface.dbcc_classguid == PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL)
                        {
                            OnDisplayDeviceArrival?.Invoke(this, EventArgs.Empty);
                            MessagingCenter.Publish(PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL);
                        }
                    }
                }
            }

            base.WndProc(ref m);
        }
    }
}
