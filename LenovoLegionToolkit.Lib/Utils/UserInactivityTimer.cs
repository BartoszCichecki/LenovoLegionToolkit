using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Timer = System.Threading.Timer;

namespace LenovoLegionToolkit.Lib.Utils;

public class UserInactivityTimer : NativeWindow
{
    private readonly IMainThreadDispatcher _mainThreadDispatcher;
    private readonly TimeSpan _timeout;
    private readonly Func<Task> _userBecameActive;
    private readonly Func<Task> _userBecameInactive;

    private readonly HOOKPROC _hookProc;

    private bool _isUserActive = true;
    private Timer? _timer;

    private HHOOK _kbHook;
    private HHOOK _mouseHook;

    public UserInactivityTimer(IMainThreadDispatcher mainThreadDispatcher, TimeSpan timeout, Func<Task> userBecameActive, Func<Task> userBecameInactive)
    {
        _mainThreadDispatcher = mainThreadDispatcher;
        _timeout = timeout;
        _userBecameActive = userBecameActive;
        _userBecameInactive = userBecameInactive;

        _hookProc = LowLevelHookProc;
    }

    public void Start() => _mainThreadDispatcher.Dispatch(() =>
    {
        CreateHandle(new CreateParams
        {
            Caption = "LenovoLegionToolkit_InactivityTimerWindow",
            Parent = new IntPtr(-3)
        });

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _hookProc, HINSTANCE.Null, 0);
        _mouseHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_MOUSE_LL, _hookProc, HINSTANCE.Null, 0);

        _timer = new Timer(Callback, null, _timeout, Timeout.InfiniteTimeSpan);
        _isUserActive = true;
    });

    public void Stop() => _mainThreadDispatcher.Dispatch(() =>
    {
        _timer?.Dispose();
        _timer = null;

        PInvoke.UnhookWindowsHookEx(_kbHook);
        PInvoke.UnhookWindowsHookEx(_mouseHook);

        _kbHook = HHOOK.Null;
        _mouseHook = HHOOK.Null;

        ReleaseHandle();
    });

    private void Callback(object? state)
    {
        Task.Run(_userBecameInactive);
        _isUserActive = false;
    }

    private LRESULT LowLevelHookProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (!_isUserActive)
            Task.Run(_userBecameActive);

        _isUserActive = true;
        _timer?.Change(_timeout, Timeout.InfiniteTimeSpan);

        return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);
    }
}
