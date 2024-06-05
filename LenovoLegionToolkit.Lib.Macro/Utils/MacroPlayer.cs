using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Extensions;
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

                    try
                    {
                        var input = ToInput(macroEvent, Screen.PrimaryScreen?.WorkingArea ?? Rectangle.Empty);
                        var result = PInvoke.SendInput(MemoryMarshal.CreateSpan(ref input, 1), Marshal.SizeOf<INPUT>());
                        if (result == 0)
                            PInvokeExtensions.ThrowIfWin32Error($"Failed to send input. Return code was {result}.");
                    }
                    catch (Exception ex)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Failed to send input for event {macroEvent}", ex);
                    }
                }
            }
        }, token);
    }

    private static INPUT ToInput(MacroEvent macroEvent, Rectangle screenArea) => macroEvent.Source switch
    {
        MacroSource.Keyboard => ToKeyboardInput(macroEvent),
        MacroSource.Mouse => ToMouseInput(macroEvent, screenArea),
        MacroSource.Unknown => throw new ArgumentException(null, nameof(macroEvent)),
        _ => throw new ArgumentOutOfRangeException(nameof(macroEvent))
    };

    private static INPUT ToKeyboardInput(MacroEvent macroEvent) => new()
    {
        type = INPUT_TYPE.INPUT_KEYBOARD,
        Anonymous = new INPUT._Anonymous_e__Union
        {
            ki = new KEYBDINPUT
            {
                wVk = (VIRTUAL_KEY)macroEvent.Key,
                dwFlags = macroEvent.Direction switch
                {
                    MacroDirection.Up => KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP,
                    _ => 0
                },
                dwExtraInfo = MAGIC_NUMBER
            }
        }
    };

    private static INPUT ToMouseInput(MacroEvent macroEvent, Rectangle screenArea) => new()
    {
        type = INPUT_TYPE.INPUT_MOUSE,
        Anonymous = new INPUT._Anonymous_e__Union
        {
            mi = new MOUSEINPUT
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
