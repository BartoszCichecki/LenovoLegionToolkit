using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Macro.Utils;

internal class MacroPlayer
{
    private const int MAGIC_NUMBER = 1337;

    private readonly ThreadSafeBool _isPlayingInterruptableSequence = new();

    private Task _playTask = Task.CompletedTask;
    private CancellationTokenSource _cancellationTokenSource = new();

    public void InterruptIfNeeded(KBDLLHOOKSTRUCT kbStruct)
    {
        if (!_isPlayingInterruptableSequence.Value)
            return;
        if (kbStruct.flags != 0)
            return;
        if (kbStruct.dwExtraInfo == MAGIC_NUMBER)
            return;

        _cancellationTokenSource.Cancel();
    }

    public async Task StartPlayingAsync(MacroSequence sequence)
    {
        await _cancellationTokenSource.CancelAsync();
        try { await _playTask; }
        catch (OperationCanceledException) { }

        _cancellationTokenSource = new();
        var token = _cancellationTokenSource.Token;

        _playTask = Task.Run(async () =>
        {
            _isPlayingInterruptableSequence.Value = sequence.InterruptOnOtherKey;

            for (var i = 0; i < sequence.RepeatCount; i++)
            {
                foreach (var macroEvent in sequence.Events ?? [])
                {
                    if (!sequence.IgnoreDelays)
                        await Task.Delay(macroEvent.Delay, token).ConfigureAwait(false);

                    token.ThrowIfCancellationRequested();

                    var input = ToInput(macroEvent, Screen.PrimaryScreen?.WorkingArea ?? Rectangle.Empty);
                    PInvoke.SendInput(MemoryMarshal.CreateSpan(ref input, 1), Marshal.SizeOf<INPUT>());
                }
            }
        }, token);
    }

    private static INPUT ToInput(MacroEvent macroEvent, Rectangle screenArea) => new()
    {
        type = macroEvent.Source switch
        {
            MacroSource.Mouse => INPUT_TYPE.INPUT_MOUSE,
            MacroSource.Keyboard => INPUT_TYPE.INPUT_KEYBOARD,
            MacroSource.Unknown => throw new ArgumentException(null, nameof(macroEvent)),
            _ => throw new ArgumentOutOfRangeException(nameof(macroEvent))
        },
        Anonymous = new INPUT._Anonymous_e__Union
        {
            ki = macroEvent.Source is not MacroSource.Keyboard
                ? default
                : new KEYBDINPUT
                {
                    wVk = (VIRTUAL_KEY)macroEvent.Key,
                    dwFlags = macroEvent.Direction switch
                    {
                        MacroDirection.Up => KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP,
                        _ => 0
                    },
                    dwExtraInfo = MAGIC_NUMBER
                },
            mi = macroEvent.Source is not MacroSource.Mouse
                ? default
                : new MOUSEINPUT
                {
                    dwFlags = (macroEvent.Direction, macroEvent.Key) switch
                    {
                        (MacroDirection.Up, 1) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP,
                        (MacroDirection.Down, 1) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN,
                        (MacroDirection.Up, 2) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP,
                        (MacroDirection.Down, 2) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN,
                        (MacroDirection.Up, 3) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEUP,
                        (MacroDirection.Down, 3) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN,
                        (MacroDirection.Up, > 0xFF) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_XUP,
                        (MacroDirection.Down, > 0xFF) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_XDOWN,
                        (MacroDirection.Wheel, _) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_WHEEL,
                        (MacroDirection.Move, _) => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE,
                        _ => 0
                    },
                    mouseData = (macroEvent.Direction, macroEvent.Key) switch
                    {
                        (MacroDirection.Up, >= 0xFF) => macroEvent.Key >> 16,
                        (MacroDirection.Down, >= 0xFF) => macroEvent.Key >> 16,
                        (MacroDirection.Wheel, _) => macroEvent.Key,
                        _ => 0
                    },
                    dx = macroEvent.Direction switch
                    {
                        MacroDirection.Move => (int)(65535.0f * (macroEvent.Point.X / (float)screenArea.Width) + 0.5f),
                        _ => 0
                    },
                    dy = macroEvent.Direction switch
                    {
                        MacroDirection.Move => (int)(65535.0f * (macroEvent.Point.Y / (float)screenArea.Height) + 0.5f),
                        _ => 0
                    },
                    dwExtraInfo = MAGIC_NUMBER
                }
        }
    };
}
