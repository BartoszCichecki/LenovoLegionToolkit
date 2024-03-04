using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Controllers;

public class SmartFnLockController(FnLockFeature feature, ApplicationSettings settings)
{
    private readonly AsyncLock _lock = new();

    private bool _ctrlDepressed;
    private bool _shiftDepressed;
    private bool _altDepressed;
    private bool _restoreFnLock;

    public void OnKeyboardEvent(nuint wParam, KBDLLHOOKSTRUCT kbStruct)
    {
        if (settings.Store.SmartFnLockFlags == 0)
            return;

        Task.Run(async () =>
        {
            try
            {
                using (await _lock.LockAsync().ConfigureAwait(false))
                    await OnKeyboardEventAsync(wParam, kbStruct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to handle keyboard event.", ex);
            }
        });
    }

    private async Task OnKeyboardEventAsync(nuint wParam, KBDLLHOOKSTRUCT kbStruct)
    {
        if (IsModifierKeyPressed(wParam, kbStruct))
        {
            if (_restoreFnLock)
                return;

            var state = await feature.GetStateAsync().ConfigureAwait(false);
            if (state == FnLockState.Off)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Disabling Fn Lock temporarily...");

            await feature.SetStateAsync(FnLockState.Off).ConfigureAwait(false);
            _restoreFnLock = true;
        }
        else if (_restoreFnLock)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Re-enabling Fn Lock...");

            await feature.SetStateAsync(FnLockState.On).ConfigureAwait(false);
            _restoreFnLock = false;
        }
    }

    private bool IsModifierKeyPressed(nuint wParam, KBDLLHOOKSTRUCT kbStruct)
    {
        var isKeyDown = wParam is PInvoke.WM_KEYDOWN or PInvoke.WM_SYSKEYDOWN;
        var vkKeyCode = (VIRTUAL_KEY)kbStruct.vkCode;

        if (vkKeyCode is VIRTUAL_KEY.VK_LCONTROL or VIRTUAL_KEY.VK_RCONTROL)
            _ctrlDepressed = isKeyDown;

        if (vkKeyCode is VIRTUAL_KEY.VK_LSHIFT or VIRTUAL_KEY.VK_RSHIFT)
            _shiftDepressed = isKeyDown;

        if (vkKeyCode is VIRTUAL_KEY.VK_LMENU or VIRTUAL_KEY.VK_RMENU)
            _altDepressed = isKeyDown;

        if (!_ctrlDepressed && !_shiftDepressed && !_altDepressed)
            return false;

        var result = false;
        var flags = settings.Store.SmartFnLockFlags;

        if (flags.HasFlag(ModifierKey.Ctrl))
            result |= _ctrlDepressed;

        if (flags.HasFlag(ModifierKey.Shift))
            result |= _shiftDepressed;

        if (flags.HasFlag(ModifierKey.Alt))
            result |= _altDepressed;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Modifier key is depressed: {result} [ctrl={_ctrlDepressed}, shift={_shiftDepressed}, alt={_altDepressed}, flags={flags}]");

        return result;
    }
}
