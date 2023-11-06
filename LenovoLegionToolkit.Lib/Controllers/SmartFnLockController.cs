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
    private bool _winDepressed;
    private bool _altDepressed;
    private bool _restoreFnLock;

    public SmartFnLockController(FnLockFeature feature, ApplicationSettings settings)
    {
        _feature = feature ?? throw new ArgumentNullException(nameof(feature));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public void OnKeyboardEvent(nuint wParam, KBDLLHOOKSTRUCT kbStruct)
    {
        if (!_settings.Store.SmartFnLock)
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
        var isKeyDown = wParam == PInvoke.WM_KEYDOWN;
        var vkKeyCode = (VIRTUAL_KEY)kbStruct.vkCode;

        if (vkKeyCode is VIRTUAL_KEY.VK_LCONTROL or VIRTUAL_KEY.VK_RCONTROL)
            _ctrlDepressed = isKeyDown;

        if (vkKeyCode is VIRTUAL_KEY.VK_LSHIFT or VIRTUAL_KEY.VK_RSHIFT)
            _shiftDepressed = isKeyDown;

        if (vkKeyCode is VIRTUAL_KEY.VK_LWIN or VIRTUAL_KEY.VK_RWIN)
            _winDepressed = isKeyDown;

        _altDepressed = kbStruct.flags.HasFlag(KBDLLHOOKSTRUCT_FLAGS.LLKHF_ALTDOWN);

        return _ctrlDepressed || _shiftDepressed || _winDepressed || _altDepressed;
    }
}
