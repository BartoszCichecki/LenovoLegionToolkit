using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
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
        public bool Equals(MacroEvent x, MacroEvent y) => x.Key == y.Key;

        public int GetHashCode(MacroEvent obj) => HashCode.Combine(obj.Key);
    }

    private readonly HashSet<MacroEvent> _rolloverCache = new(new MacroEventEqualityComparer());

    private readonly HOOKPROC _kbProc;

    private HHOOK _kbHook;
    private TimeSpan _timeFromLastEvent;
    private bool _interrupted;

    public bool IsRecording => _kbHook != HHOOK.Null;

    public event EventHandler<ReceivedEventArgs>? Received;
    public event EventHandler<StoppedEventArgs>? Stopped;

    public MacroRecorder()
    {
        _kbProc = LowLevelKeyboardProc;
    }

    public void StartRecording()
    {
        if (_kbHook != HHOOK.Null)
            return;

        _interrupted = false;
        _timeFromLastEvent = TimeSpan.Zero;

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _kbProc, HINSTANCE.Null, 0);
        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
    }

    public void StopRecording()
    {
        if (!IsRecording)
            return;

        var wasInterrupted = _interrupted;

        SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        PInvoke.UnhookWindowsHookEx(_kbHook);
        _kbHook = default;

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

        if (macroEvent.Value.Direction == MacroDirection.Down && _rolloverCache.Contains(macroEvent.Value))
            return result;

        Received?.Invoke(this, new ReceivedEventArgs { MacroEvent = macroEvent.Value });

        _timeFromLastEvent = TimeSpan.FromMilliseconds(kbStruct.time);

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
}
