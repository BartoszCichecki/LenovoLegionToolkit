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

public class SmartFnLockController
{
    private readonly AsyncLock _lock = new();

    private readonly FnLockFeature _feature;
    private readonly ApplicationSettings _settings;

    private bool _ctrlDepressed;
    private bool _shiftDepressed;
    private bool _altDepressed;
    private bool _restoreFnLock;

    public SmartFnLockController(FnLockFeature feature, ApplicationSettings settings)
    {
        _feature = feature ?? throw new ArgumentNullException(nameof(feature));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public void OnKeyboardEvent(nuint wParam, KBDLLHOOKSTRUCT kbStruct)
    {
        if (_settings.Store.SmartFnLockFlags == ModifierKey.None)
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

            var state = await _feature.GetStateAsync().ConfigureAwait(false);
            if (state == FnLockState.Off)
                return;

            await _feature.SetStateAsync(FnLockState.Off).ConfigureAwait(false);
            _restoreFnLock = true;
        }
        else if (_restoreFnLock)
        {
            await _feature.SetStateAsync(FnLockState.On).ConfigureAwait(false);
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

        var result = false;

        if (_settings.Store.SmartFnLockFlags.HasFlag(ModifierKey.Ctrl))
            result |= _ctrlDepressed;

        if (_settings.Store.SmartFnLockFlags.HasFlag(ModifierKey.Shift))
            result |= _shiftDepressed;

        if (_settings.Store.SmartFnLockFlags.HasFlag(ModifierKey.Alt))
            result |= _altDepressed;

        return result;
    }
}
