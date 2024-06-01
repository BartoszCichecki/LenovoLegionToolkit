using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
        if (kbStruct.dwExtraInfo == 1337)
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

                    var input = ToInput(macroEvent);
                    PInvoke.SendInput(MemoryMarshal.CreateSpan(ref input, 1), Marshal.SizeOf<INPUT>());
                }
            }
        }, token);
    }

    private static INPUT ToInput(MacroEvent macroEvent) => new()
    {
        type = INPUT_TYPE.INPUT_KEYBOARD,
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
                }
        }
    };
}
