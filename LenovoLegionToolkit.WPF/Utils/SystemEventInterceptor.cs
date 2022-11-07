using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Foundation;
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
        private readonly HOOKPROC _kbProc;
        private readonly HHOOK _kbHook;

        public event EventHandler? OnTaskbarCreated;
        public event EventHandler? OnDisplayDeviceArrival;
        public event EventHandler? OnResumed;

        public SystemEventInterceptor(Window parent)
        {
            var ptr = new WindowInteropHelper(parent).Handle;
            _safeHandle = new SafeAccessTokenHandle(ptr);

            _kbProc = LowLevelKeyboardProc;
            _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _kbProc, HINSTANCE.Null, 0);

            _taskbarCreatedMessageId = RegisterTaskbarCreatedMessage();
            _displayArrivalHandle = RegisterDisplayArrival(_safeHandle);

            _powerNotificationHandle = PInvoke.RegisterPowerSettingNotification(_safeHandle, PInvoke.GUID_MONITOR_POWER_ON, 0);

            parent.Closed += Parent_Closed;

            AssignHandle(ptr);
        }

        private void Parent_Closed(object? sender, EventArgs e)
        {
            PInvoke.UnhookWindowsHookEx(_kbHook);

            PInvoke.UnregisterDeviceNotification(_displayArrivalHandle);
            PInvoke.UnregisterPowerSettingNotification(new HPOWERNOTIFY(_powerNotificationHandle.DangerousGetHandle()));

            _powerNotificationHandle.DangerousRelease();

            ReleaseHandle();
        }

        private static uint RegisterTaskbarCreatedMessage()
        {
            var message = PInvoke.RegisterWindowMessage("TaskbarCreated");
            PInvoke.ChangeWindowMessageFilter(message, CHANGE_WINDOW_MESSAGE_FILTER_FLAGS.MSGFLT_ADD);
            return message;
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
                        var data = new byte[1];
                        Marshal.Copy(new IntPtr(powerBroadcastSettings.Data.Value), data, 0, 1);
                        if (data[0] == 1)
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

        private LRESULT LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
        {
            if (nCode == PInvoke.HC_ACTION && wParam.Value == PInvoke.WM_KEYUP)
            {
                var kbStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(new IntPtr(lParam.Value));

                if (kbStruct.vkCode == PInvokeExtensions.VK_CAPITAL)
                {
                    var isOn = Keyboard.IsKeyToggled(Key.CapsLock);
                    MessagingCenter.Publish(new Notification(isOn ? NotificationType.CapsLockOn : NotificationType.CapsLockOff, NotificationDuration.Short));
                }

                if (kbStruct.vkCode == PInvokeExtensions.VK_NUMLOCK)
                {
                    var isOn = Keyboard.IsKeyToggled(Key.NumLock);
                    MessagingCenter.Publish(new Notification(isOn ? NotificationType.NumLockOn : NotificationType.NumLockOff, NotificationDuration.Short));
                }
            }

            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);
        }
    }
}
