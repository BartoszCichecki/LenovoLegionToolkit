using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

// ReSharper disable once MemberCanBeMadeStatic.Global
#pragma warning disable CA1822 // Mark members as static

namespace LenovoLegionToolkit.Lib.Macro.Utils;

internal class MacroPlayer
{
    public async Task PlayAsync(MacroSequence sequence, CancellationToken token)
    {
        for (var i = 0; i < sequence.RepeatCount; i++)
        {
            foreach (var macroEvent in sequence.Events ?? [])
            {
                if (!sequence.IgnoreDelays)
                    await Task.Delay(macroEvent.Delay, token).ConfigureAwait(false);

                token.ThrowIfCancellationRequested();

                PInvoke.SendInput(ToInput(macroEvent), Marshal.SizeOf<INPUT>());
            }

            token.ThrowIfCancellationRequested();
        }
    }

    private static Span<INPUT> ToInput(MacroEvent macroEvent)
    {
        var input = new INPUT
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
                    }
                }
            }
        };

        return new Span<INPUT>([input]);
    }
}
