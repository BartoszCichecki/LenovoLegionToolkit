using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Macro.Utils;
using NeoSmart.AsyncLock;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LenovoLegionToolkit.Lib.Macro;

public class MacroController
{
    private static readonly uint[] AllowedRange = [0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69];

    private readonly AsyncLock _ioLock = new();

    private readonly MacroRecorder _recorder = new();
    private readonly MacroPlayer _player = new();

    private readonly HOOKPROC _kbProc;
    private readonly MacroSettings _settings;

    private HHOOK _kbHook;
    private CancellationTokenSource? _cancellationTokenSource;

    public MacroController(MacroSettings settings)
    {
        _settings = settings;

        _kbProc = LowLevelKeyboardProc;
    }

    public bool IsEnabled => _settings.Store.IsEnabled;

    public async Task SetEnabledAsync(bool enabled)
    {
        using (await _ioLock.LockAsync().ConfigureAwait(false))
        {
            _settings.Store.IsEnabled = enabled;
            _settings.SynchronizeStore();
        }
    }
    public void Start()
    {
        if (_kbHook != default)
            return;

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _kbProc, HINSTANCE.Null, 0);
    }

    public void Stop()
    {
        PInvoke.UnhookWindowsHookEx(_kbHook);
        _kbHook = default;
    }

    private unsafe LRESULT LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode != PInvoke.HC_ACTION)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        if (!IsEnabled)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        ref var kbStruct = ref Unsafe.AsRef<KBDLLHOOKSTRUCT>((void*)lParam.Value);

        var shouldRun = kbStruct.flags == 0;
        shouldRun &= AllowedRange.Contains(kbStruct.vkCode);
        shouldRun &= _settings.Store.Sequences.ContainsKey(kbStruct.vkCode);

        if (!shouldRun)
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);

        var sequence = _settings.Store.Sequences[kbStruct.vkCode];

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        Play(sequence, token);

        return new LRESULT(96);

    }

    private void Play(MacroSequence sequence, CancellationToken token) => Task.Run(() => _player.PlayAsync(sequence, token), token);
}
