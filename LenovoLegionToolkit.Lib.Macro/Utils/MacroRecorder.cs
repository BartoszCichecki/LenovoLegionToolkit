using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Macro.Utils;

internal class MacroRecorder
{
    public class ReceivedEventArgs : EventArgs
    {
        public MacroEvent MacroEvent { get; init; }
    }

    public class StoppedEventArgs : EventArgs
    {
        public bool Interrupted { get; init; }
    }

    private class MacroEventEqualityComparer : IEqualityComparer<MacroEvent>
    {
        public bool Equals(MacroEvent x, MacroEvent y) => x.Source == y.Source && x.Key == y.Key;

        public int GetHashCode(MacroEvent obj) => HashCode.Combine(obj.Source, obj.Key);
    }

    private readonly HashSet<MacroEvent> _rolloverCache = new(new MacroEventEqualityComparer());

    private readonly HOOKPROC _kbProc;
    private readonly HOOKPROC _mouseProc;

    private HHOOK _kbHook;
    private HHOOK _mouseHook;
    private TimeSpan _timeFromLastEvent;
    private bool _interrupted;

    public bool IsRecording => _kbHook != HHOOK.Null && _mouseHook != HHOOK.Null;

    public event EventHandler<ReceivedEventArgs>? Received;
    public event EventHandler<StoppedEventArgs>? Stopped;

    public MacroRecorder()
    {
        _kbProc = LowLevelKeyboardProc;
        _mouseProc = LowLevelMouseProc;
    }

    public void StartRecording()
    {
        if (_kbHook != HHOOK.Null)
            return;

        _interrupted = false;
        _timeFromLastEvent = TimeSpan.Zero;

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _kbProc, HINSTANCE.Null, 0);
        _mouseHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_MOUSE_LL, _mouseProc, HINSTANCE.Null, 0);
        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
    }

    public void StopRecording()
    {
        if (!IsRecording)
            return;

        var wasInterrupted = _interrupted;

        SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;

        PInvoke.UnhookWindowsHookEx(_kbHook);
        PInvoke.UnhookWindowsHookEx(_mouseHook);

        _kbHook = default;
        _mouseHook = default;

        _interrupted = false;
        _timeFromLastEvent = TimeSpan.Zero;

        Stopped?.Invoke(this, new() { Interrupted = wasInterrupted });
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        var interrupt = new[]
        {
            SessionSwitchReason.ConsoleDisconnect,
            SessionSwitchReason.SessionLock,
            SessionSwitchReason.SessionLogoff,
            SessionSwitchReason.SessionRemoteControl,
            SessionSwitchReason.RemoteDisconnect
        }.Contains(e.Reason);

        if (!interrupt)
            return;

        _interrupted = true;
        StopRecording();
    }

    private unsafe LRESULT LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode < 0)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        var result = new LRESULT(69);

        ref var kbStruct = ref Unsafe.AsRef<KBDLLHOOKSTRUCT>((void*)lParam.Value);

        var macroEvent = ConvertToMacroEvent(wParam, kbStruct, _timeFromLastEvent);

        if (!macroEvent.HasValue)
            return result;

        if (macroEvent.Value.IsUndefined())
            return result;

        if (macroEvent.Value.Direction == MacroDirection.Down)
        {
            if (macroEvent.Value.Key == (ulong)VIRTUAL_KEY.VK_ESCAPE)
            {
                StopRecording();
                return result;
            }

            if (_rolloverCache.Contains(macroEvent.Value))
                return result;
        }

        Received?.Invoke(this, new ReceivedEventArgs { MacroEvent = macroEvent.Value });

        _timeFromLastEvent = TimeSpan.FromMilliseconds(kbStruct.time);

        if (macroEvent.Value.Direction == MacroDirection.Down)
            _rolloverCache.Add(macroEvent.Value);
        else
            _rolloverCache.Remove(macroEvent.Value);

        return result;
    }

    private unsafe LRESULT LowLevelMouseProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode < 0)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        var result = new LRESULT(96);

        ref var mouseStruct = ref Unsafe.AsRef<MSLLHOOKSTRUCT>((void*)lParam.Value);

        var macroEvent = ConvertToMacroEvent(wParam, mouseStruct, _timeFromLastEvent);

        if (!macroEvent.HasValue)
            return result;

        if (macroEvent.Value.IsUndefined())
            return result;

        if (macroEvent.Value.Direction == MacroDirection.Down && _rolloverCache.Contains(macroEvent.Value))
            return result;

        Received?.Invoke(this, new ReceivedEventArgs { MacroEvent = macroEvent.Value });

        _timeFromLastEvent = TimeSpan.FromMilliseconds(mouseStruct.time);

        if (macroEvent.Value.Direction == MacroDirection.Down)
            _rolloverCache.Add(macroEvent.Value);
        else
            _rolloverCache.Remove(macroEvent.Value);

        return result;
    }

    private static MacroEvent? ConvertToMacroEvent(WPARAM wParam, KBDLLHOOKSTRUCT kbStruct, TimeSpan timeFromLastEvent)
    {
        if (timeFromLastEvent == TimeSpan.Zero)
            timeFromLastEvent = TimeSpan.FromMilliseconds(kbStruct.time);

        var delay = TimeSpan.FromMilliseconds(kbStruct.time) - timeFromLastEvent;

        var macroEvent = new MacroEvent
        {
            Source = MacroSource.Keyboard,
            Direction = (uint)wParam switch
            {
                PInvoke.WM_KEYUP or PInvoke.WM_SYSKEYUP => MacroDirection.Up,
                PInvoke.WM_KEYDOWN or PInvoke.WM_SYSKEYDOWN => MacroDirection.Down,
                _ => MacroDirection.Unknown
            },
            Key = kbStruct.vkCode,
            Delay = delay
        };

        return macroEvent;
    }

    private static MacroEvent? ConvertToMacroEvent(WPARAM wParam, MSLLHOOKSTRUCT mouseStruct, TimeSpan timeFromLastEvent)
    {
        if (timeFromLastEvent == TimeSpan.Zero)
            timeFromLastEvent = TimeSpan.FromMilliseconds(mouseStruct.time);

        var delay = TimeSpan.FromMilliseconds(mouseStruct.time) - timeFromLastEvent;

        var macroEvent = new MacroEvent
        {
            Source = MacroSource.Mouse,
            Direction = (uint)wParam switch
            {
                PInvoke.WM_LBUTTONUP or PInvoke.WM_RBUTTONUP or PInvoke.WM_MBUTTONUP or PInvoke.WM_XBUTTONUP => MacroDirection.Up,
                PInvoke.WM_LBUTTONDOWN or PInvoke.WM_RBUTTONDOWN or PInvoke.WM_MBUTTONDOWN or PInvoke.WM_XBUTTONDOWN => MacroDirection.Down,
                _ => MacroDirection.Unknown
            },
            Key = (uint)wParam switch
            {
                PInvoke.WM_LBUTTONUP or PInvoke.WM_LBUTTONDOWN => 1,
                PInvoke.WM_RBUTTONUP or PInvoke.WM_RBUTTONDOWN => 2,
                PInvoke.WM_MBUTTONUP or PInvoke.WM_MBUTTONDOWN => 3,
                PInvoke.WM_XBUTTONUP or PInvoke.WM_XBUTTONDOWN => mouseStruct.mouseData,
                _ => 0
            },
            Delay = delay
        };

        return macroEvent;
    }
}
