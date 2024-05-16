using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

// ReSharper disable all MemberCanBeMadeStatic.Global
#pragma warning disable CA1822 // Mark members as static

namespace LenovoLegionToolkit.Lib.Macro.Utils;

internal class MacroPlayer
{
    private const int MAGIC_NUMBER = 1337;

    public bool PlayedEvent(UIntPtr dwExtraInfo) => dwExtraInfo == 1337;

    public async Task PlayAsync(MacroSequence sequence, CancellationToken token)
    {
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
