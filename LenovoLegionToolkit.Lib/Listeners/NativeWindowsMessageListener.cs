using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;
using Windows.Win32.System.SystemServices;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Listeners;


public unsafe class NativeWindowsMessageListener : NativeWindow, IListener<NativeWindowsMessage>
{
    private readonly IGPUModeFeature _igpuModeFeature;

    private readonly HOOKPROC _kbProc;

    private void* _displayArrivalHandle;
    private void* _devInterfaceMonitorHandle;
    private HHOOK _kbHook;

    public event EventHandler<NativeWindowsMessage>? Changed;

    public NativeWindowsMessageListener(IGPUModeFeature igpuModeFeature)
    {
        _igpuModeFeature = igpuModeFeature ?? throw new ArgumentNullException(nameof(igpuModeFeature));

        _kbProc = LowLevelKeyboardProc;
    }

    public Task StartAsync()
    {
        CreateHandle(new CreateParams
        {
            Caption = "LenovoLegionToolkit_MessageWindow",
            Parent = new IntPtr(-3)
        });

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _kbProc, HINSTANCE.Null, 0);

        _displayArrivalHandle = RegisterDeviceNotification(Handle, PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL);
        _devInterfaceMonitorHandle = RegisterDeviceNotification(Handle, PInvoke.GUID_DEVINTERFACE_MONITOR);

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        PInvoke.UnhookWindowsHookEx(_kbHook);

        PInvoke.UnregisterDeviceNotification(_displayArrivalHandle);
        PInvoke.UnregisterDeviceNotification(_devInterfaceMonitorHandle);

        _kbHook = HHOOK.Null;
        _displayArrivalHandle = null;
        _devInterfaceMonitorHandle = null;

        ReleaseHandle();

        return Task.CompletedTask;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == PInvoke.WM_DEVICECHANGE && m.LParam != IntPtr.Zero)
        {
            var devBroadcastHdr = Marshal.PtrToStructure<DEV_BROADCAST_HDR>(m.LParam);
            if (devBroadcastHdr.dbch_devicetype == DEV_BROADCAST_HDR_DEVICE_TYPE.DBT_DEVTYP_DEVICEINTERFACE)
            {
                var devBroadcastDeviceInterface = Marshal.PtrToStructure<DEV_BROADCAST_DEVICEINTERFACE_W>(m.LParam);
                if (devBroadcastDeviceInterface.dbcc_classguid == PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL)
                {
                    OnDisplayDeviceArrival();
                }

                if (devBroadcastDeviceInterface.dbcc_classguid == PInvoke.GUID_DEVINTERFACE_MONITOR)
                {
                    if (m.WParam.ToInt32() == PInvoke.DBT_DEVICEARRIVAL)
                        OnMonitorConnected();

                    if (m.WParam.ToInt32() == PInvoke.DBT_DEVICEREMOVECOMPLETE)
                        OnMonitorDisconnected();
                }
            }
        }

        base.WndProc(ref m);
    }

    private void OnMonitorConnected()
    {
        Changed?.Invoke(this, NativeWindowsMessage.MonitorConnected);
    }

    private void OnMonitorDisconnected()
    {
        Changed?.Invoke(this, NativeWindowsMessage.MonitorDisconnected);
    }

    private void OnDisplayDeviceArrival()
    {
        Task.Run(_igpuModeFeature.NotifyAsync);

        Changed?.Invoke(this, NativeWindowsMessage.OnDisplayDeviceArrival);
    }

    private LRESULT LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode == PInvoke.HC_ACTION && wParam.Value == PInvoke.WM_KEYUP)
        {
            var kbStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(new IntPtr(lParam.Value));

            if (kbStruct.vkCode == PInvokeExtensions.VK_CAPITAL)
            {
                var isOn = (PInvoke.GetKeyState((int)PInvokeExtensions.VK_CAPITAL) & 0x1) != 0;
                MessagingCenter.Publish(new Notification(isOn ? NotificationType.CapsLockOn : NotificationType.CapsLockOff, NotificationDuration.Short));
            }

            if (kbStruct.vkCode == PInvokeExtensions.VK_NUMLOCK)
            {
                var isOn = (PInvoke.GetKeyState((int)PInvokeExtensions.VK_NUMLOCK) & 0x1) != 0;
                MessagingCenter.Publish(new Notification(isOn ? NotificationType.NumLockOn : NotificationType.NumLockOff, NotificationDuration.Short));
            }
        }

        return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);
    }

    private static void* RegisterDeviceNotification(IntPtr handle, Guid classGuid)
    {
        var ptr = IntPtr.Zero;
        try
        {
            var str = new DEV_BROADCAST_DEVICEINTERFACE_W();
            str.dbcc_size = (uint)Marshal.SizeOf(str);
            str.dbcc_devicetype = (uint)DEV_BROADCAST_HDR_DEVICE_TYPE.DBT_DEVTYP_DEVICEINTERFACE;
            str.dbcc_classguid = classGuid;
            ptr = Marshal.AllocHGlobal(Marshal.SizeOf(str));
            Marshal.StructureToPtr(str, ptr, true);
            return PInvoke.RegisterDeviceNotification(new HANDLE(handle), ptr.ToPointer(), POWER_SETTING_REGISTER_NOTIFICATION_FLAGS.DEVICE_NOTIFY_WINDOW_HANDLE);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
