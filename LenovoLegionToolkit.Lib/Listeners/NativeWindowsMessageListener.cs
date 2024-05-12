using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Features.Hybrid.Notify;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Listeners;

public class NativeWindowsMessageListener : NativeWindow, IListener<NativeWindowsMessageListener.ChangedEventArgs>
{
    public class ChangedEventArgs(NativeWindowsMessage message) : EventArgs
    {
        public NativeWindowsMessage Message { get; } = message;
    }

    private readonly IMainThreadDispatcher _mainThreadDispatcher;
    private readonly DGPUNotify _dgpuNotify;
    private readonly SmartFnLockController _smartFnLockController;
    private readonly PowerModeFeature _powerModeFeature;

    private readonly HOOKPROC _kbProc;

    private readonly TaskCompletionSource _isMonitorOnTaskCompletionSource = new();
    private readonly TaskCompletionSource _isLidOpenTaskCompletionSource = new();

    private HDEVNOTIFY _displayArrivalHandle;
    private HDEVNOTIFY _devInterfaceMonitorHandle;
    private HPOWERNOTIFY _consoleDisplayStateNotificationHandle;
    private HPOWERNOTIFY _lidSwitchStateChangeNotificationHandle;
    private HPOWERNOTIFY _powerSavingStateChangeNotificationHandle;
    private HHOOK _kbHook;

    public bool IsMonitorOn { get; private set; }
    public bool IsLidOpen { get; private set; }

    public event EventHandler<ChangedEventArgs>? Changed;

    public NativeWindowsMessageListener(IMainThreadDispatcher mainThreadDispatcher, DGPUNotify dgpuNotify, SmartFnLockController smartFnLockController, PowerModeFeature powerModeFeature)
    {
        _mainThreadDispatcher = mainThreadDispatcher;
        _dgpuNotify = dgpuNotify;
        _smartFnLockController = smartFnLockController;
        _powerModeFeature = powerModeFeature;

        _kbProc = LowLevelKeyboardProc;
    }

    public async Task TurnOffMonitorAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
        await _mainThreadDispatcher.DispatchAsync(() =>
        {
            PInvoke.SendMessage(new HWND(Handle), PInvoke.WM_SYSCOMMAND, new WPARAM(PInvoke.SC_MONITORPOWER), new LPARAM(2));
            return Task.CompletedTask;
        }).ConfigureAwait(false);
    }

    public Task StartAsync() => _mainThreadDispatcher.DispatchAsync(() =>
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
        _powerSavingStateChangeNotificationHandle = RegisterPowerNotification(PInvoke.GUID_POWER_SAVING_STATUS);

        return WaitForInit();
    });

    public Task StopAsync() => _mainThreadDispatcher.DispatchAsync(() =>
    {
        PInvoke.UnhookWindowsHookEx(_kbHook);

        PInvoke.UnregisterDeviceNotification(_displayArrivalHandle);
        PInvoke.UnregisterDeviceNotification(_devInterfaceMonitorHandle);
        PInvoke.UnregisterPowerSettingNotification(_consoleDisplayStateNotificationHandle);
        PInvoke.UnregisterPowerSettingNotification(_lidSwitchStateChangeNotificationHandle);
        PInvoke.UnregisterPowerSettingNotification(_powerSavingStateChangeNotificationHandle);

        _kbHook = default;
        _displayArrivalHandle = default;
        _devInterfaceMonitorHandle = default;
        _consoleDisplayStateNotificationHandle = default;

        ReleaseHandle();

        return Task.CompletedTask;
    });

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

            if (str.PowerSetting == PInvoke.GUID_POWER_SAVING_STATUS && str.Data[0] == 0)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Event received: Battery Saver enabled");

                OnBatterySaverEnabled();
            }
        }

        base.WndProc(ref m);
    }

    private async Task WaitForInit()
    {
        var delayTask = Task.Delay(TimeSpan.FromSeconds(3));
        var task = Task.WhenAll(
            _isMonitorOnTaskCompletionSource.Task,
            _isLidOpenTaskCompletionSource.Task
        );

        var completed = await Task.WhenAny(task, delayTask).ConfigureAwait(false);

        if (completed == delayTask)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Delay expired, state might be inconsistent! [IsMonitorOn={IsMonitorOn}, IsLidOpen={IsLidOpen}]");
        }
    }

    private void OnMonitorOn()
    {
        IsMonitorOn = true;
        _isMonitorOnTaskCompletionSource.TrySetResult();

        RaiseChanged(NativeWindowsMessage.MonitorOn);
    }

    private void OnMonitorOff()
    {
        IsMonitorOn = false;
        _isMonitorOnTaskCompletionSource.TrySetResult();

        RaiseChanged(NativeWindowsMessage.MonitorOff);
    }

    private void OnLidOpened()
    {
        IsLidOpen = true;
        _isLidOpenTaskCompletionSource.TrySetResult();

        RaiseChanged(NativeWindowsMessage.LidOpened);
    }

    private void OnLidClosed()
    {
        IsLidOpen = false;
        _isLidOpenTaskCompletionSource.TrySetResult();

        RaiseChanged(NativeWindowsMessage.LidClosed);
    }

    private void OnBatterySaverEnabled()
    {
        Task.Run(_powerModeFeature.EnsureCorrectWindowsPowerSettingsAreSetAsync);

        RaiseChanged(NativeWindowsMessage.BatterySaverEnabled);
    }

    private void OnMonitorConnected()
    {
        RaiseChanged(NativeWindowsMessage.MonitorConnected);
    }

    private void OnMonitorDisconnected()
    {
        RaiseChanged(NativeWindowsMessage.MonitorDisconnected);
    }

    private void OnDisplayDeviceArrival()
    {
        Task.Run(async () =>
        {
            if (await _dgpuNotify.IsSupportedAsync().ConfigureAwait(false))
                await _dgpuNotify.NotifyAsync().ConfigureAwait(false);
        });

        RaiseChanged(NativeWindowsMessage.OnDisplayDeviceArrival);
    }

    private void RaiseChanged(NativeWindowsMessage message) => Changed?.Invoke(this, new ChangedEventArgs(message));

    private LRESULT LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode != PInvoke.HC_ACTION)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        var kbStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(new IntPtr(lParam.Value));

        _smartFnLockController.OnKeyboardEvent(wParam.Value, kbStruct);

        if (wParam.Value != PInvoke.WM_KEYUP)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        if (kbStruct.vkCode == (ulong)VIRTUAL_KEY.VK_CAPITAL)
        {
            var isOn = (PInvoke.GetKeyState((int)VIRTUAL_KEY.VK_CAPITAL) & 0x1) != 0;
            var type = isOn ? NotificationType.CapsLockOn : NotificationType.CapsLockOff;
            MessagingCenter.Publish(new Notification(type));
        }

        if (kbStruct.vkCode == (ulong)VIRTUAL_KEY.VK_NUMLOCK)
        {
            var isOn = (PInvoke.GetKeyState((int)VIRTUAL_KEY.VK_NUMLOCK) & 0x1) != 0;
            var type = isOn ? NotificationType.NumLockOn : NotificationType.NumLockOff;
            MessagingCenter.Publish(new Notification(type));
        }

        return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);
    }

    private static unsafe HDEVNOTIFY RegisterDeviceNotification(IntPtr handle, Guid classGuid)
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
            return PInvoke.RegisterDeviceNotification(new HANDLE(handle), ptr.ToPointer(), REGISTER_NOTIFICATION_FLAGS.DEVICE_NOTIFY_WINDOW_HANDLE);
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
