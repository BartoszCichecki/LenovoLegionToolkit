using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private class MacroEventEqualityComparer : IEqualityComparer<MacroEvent>
    {
        public bool Equals(MacroEvent x, MacroEvent y) => x.Key == y.Key;

        public int GetHashCode(MacroEvent obj) => HashCode.Combine(obj.Key);
    }

    private readonly HashSet<MacroEvent> _rolloverCache = new(new MacroEventEqualityComparer());

    private readonly HOOKPROC _kbProc;

    private HHOOK _kbHook;
    private TimeSpan _timeFromLastEvent;

    public bool IsRecording => _kbHook != HHOOK.Null;

    public event EventHandler<ReceivedEventArgs>? Received;

    public MacroRecorder()
    {
        _kbProc = LowLevelKeyboardProc;
    }

    public void StartRecording()
    {
        if (_kbHook != HHOOK.Null)
            return;

        _timeFromLastEvent = TimeSpan.Zero;

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _kbProc, HINSTANCE.Null, 0);
    }

    public void StopRecording()
    {
        PInvoke.UnhookWindowsHookEx(_kbHook);
        _kbHook = default;

        _timeFromLastEvent = TimeSpan.Zero;
    }

    private unsafe LRESULT LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode < 0)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        // Returning a value greater than zero to prevent other hooks from handling the keypress
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
