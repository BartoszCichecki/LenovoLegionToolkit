using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;
using Windows.Win32.System.SystemServices;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Listeners;

public class NativeWindowsMessageListener : NativeWindow, IListener<NativeWindowsMessage>
{
    private readonly IGPUModeFeature _igpuModeFeature;

    private readonly HOOKPROC _kbProc;

    private readonly TaskCompletionSource _isMonitorOnTaskCompletionSource = new();
    private readonly TaskCompletionSource _isLidOpenTaskCompletionSource = new();

    private unsafe void* _displayArrivalHandle;
    private unsafe void* _devInterfaceMonitorHandle;
    private HPOWERNOTIFY _consoleDisplayStateNotificationHandle;
    private HPOWERNOTIFY _lidSwitchStateChangeNotificationHandle;
    private HHOOK _kbHook;

    public bool IsMonitorOn { get; private set; }
    public bool IsLidOpen { get; private set; }

    public event EventHandler<NativeWindowsMessage>? Changed;

    public NativeWindowsMessageListener(IGPUModeFeature igpuModeFeature)
    {
        _igpuModeFeature = igpuModeFeature ?? throw new ArgumentNullException(nameof(igpuModeFeature));

        _kbProc = LowLevelKeyboardProc;
    }

    public async Task TurnOffMonitorAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        PInvoke.SendMessage(new HWND(Handle), PInvoke.WM_SYSCOMMAND, new WPARAM(PInvoke.SC_MONITORPOWER), new LPARAM(2));
    }

    public unsafe Task StartAsync()
    {
        CreateHandle(new CreateParams
        {
            Caption = "LenovoLegionToolkit_MessageWindow",
            Parent = new IntPtr(-3)
        });

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _kbProc, HINSTANCE.Null, 0);

        _displayArrivalHandle = RegisterDeviceNotification(Handle, PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL);
        _devInterfaceMonitorHandle = RegisterDeviceNotification(Handle, PInvoke.GUID_DEVINTERFACE_MONITOR);
        _consoleDisplayStateNotificationHandle = RegisterPowerNotification(PInvoke.GUID_CONSOLE_DISPLAY_STATE);
        _lidSwitchStateChangeNotificationHandle = RegisterPowerNotification(PInvoke.GUID_LIDSWITCH_STATE_CHANGE);

        return Task.WhenAll(
            _isMonitorOnTaskCompletionSource.Task,
            _isLidOpenTaskCompletionSource.Task
        );
    }

    public unsafe Task StopAsync()
    {
        PInvoke.UnhookWindowsHookEx(_kbHook);

        PInvoke.UnregisterDeviceNotification(_displayArrivalHandle);
        PInvoke.UnregisterDeviceNotification(_devInterfaceMonitorHandle);
        PInvoke.UnregisterPowerSettingNotification(_consoleDisplayStateNotificationHandle);
        PInvoke.UnregisterPowerSettingNotification(_lidSwitchStateChangeNotificationHandle);

        _kbHook = HHOOK.Null;
        _displayArrivalHandle = null;
        _devInterfaceMonitorHandle = null;
        _consoleDisplayStateNotificationHandle = HPOWERNOTIFY.Null;

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
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Event received: Display Device Arrival");

                    OnDisplayDeviceArrival();
                }

                if (devBroadcastDeviceInterface.dbcc_classguid == PInvoke.GUID_DEVINTERFACE_MONITOR)
                {
                    var state = (uint)m.WParam.ToInt32();
                    switch (state)
                    {
                        case PInvoke.DBT_DEVICEARRIVAL:
                            {
                                if (Log.Instance.IsTraceEnabled)
                                    Log.Instance.Trace($"Event received: Monitor Connected");

                                OnMonitorConnected();
                                break;
                            }
                        case PInvoke.DBT_DEVICEREMOVECOMPLETE:
                            {
                                if (Log.Instance.IsTraceEnabled)
                                    Log.Instance.Trace($"Event received: Monitor Disconnected");

                                OnMonitorDisconnected();
                                break;
                            }
                    }
                }
            }
        }

        if (m.Msg == PInvoke.WM_POWERBROADCAST && m.WParam == (IntPtr)PInvoke.PBT_POWERSETTINGCHANGE && m.LParam != IntPtr.Zero)
        {
            var str = Marshal.PtrToStructure<POWERBROADCAST_SETTING>(m.LParam);

            if (str.PowerSetting == PInvoke.GUID_CONSOLE_DISPLAY_STATE)
            {
                var state = (PInvokeExtensions.CONSOLE_DISPLAY_STATE)str.Data[0];
                switch (state)
                {
                    case PInvokeExtensions.CONSOLE_DISPLAY_STATE.On:
                        {
                            if (Log.Instance.IsTraceEnabled)
                                Log.Instance.Trace($"Event received: Monitor On");

                            OnMonitorOn();
                            break;
                        }
                    case PInvokeExtensions.CONSOLE_DISPLAY_STATE.Off:
                        {
                            if (Log.Instance.IsTraceEnabled)
                                Log.Instance.Trace($"Event received: Monitor Off");

                            OnMonitorOff();
                            break;
                        }
                }
            }

            if (str.PowerSetting == PInvoke.GUID_LIDSWITCH_STATE_CHANGE)
            {
                var isOpened = str.Data[0] != 0;
                if (isOpened)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Event received: Lid Opened");

                    OnLidOpened();
                }
                else
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Event received: Lid Closed");

                    OnLidClosed();
                }
            }
        }

        base.WndProc(ref m);
    }

    private void OnMonitorOn()
    {
        IsMonitorOn = true;
        _isMonitorOnTaskCompletionSource.TrySetResult();

        Changed?.Invoke(this, NativeWindowsMessage.MonitorOn);
    }

    private void OnMonitorOff()
    {
        IsMonitorOn = false;
        _isMonitorOnTaskCompletionSource.TrySetResult();

        Changed?.Invoke(this, NativeWindowsMessage.MonitorOff);
    }

    private void OnLidOpened()
    {
        IsLidOpen = true;
        _isLidOpenTaskCompletionSource.TrySetResult();

        Changed?.Invoke(this, NativeWindowsMessage.LidOpened);
    }

    private void OnLidClosed()
    {
        IsLidOpen = false;
        _isLidOpenTaskCompletionSource.TrySetResult();

        Changed?.Invoke(this, NativeWindowsMessage.LidClosed);
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
        Task.Run(async () =>
        {
            if (await _igpuModeFeature.IsSupportedAsync().ConfigureAwait(false))
                await _igpuModeFeature.NotifyAsync().ConfigureAwait(false);
        });

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

    private static unsafe void* RegisterDeviceNotification(IntPtr handle, Guid classGuid)
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

    private unsafe HPOWERNOTIFY RegisterPowerNotification(Guid guid)
    {
        return PInvoke.RegisterPowerSettingNotification(new HANDLE(Handle), &guid, 0);
    }
}
