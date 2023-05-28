using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Timer = System.Threading.Timer;

namespace LenovoLegionToolkit.Lib.Automation.Listeners;

public class UserInactivityListener : NativeWindow, IListener<(TimeSpan, int)>
{
    private readonly TimeSpan _timerResolution = TimeSpan.FromSeconds(10);
    private readonly object _lock = new();

    private readonly IMainThreadDispatcher _mainThreadDispatcher;

    private readonly HOOKPROC _hookProc;

    private uint _tickCount;
    private Timer? _timer;

    private HHOOK _kbHook;
    private HHOOK _mouseHook;

    public event EventHandler<(TimeSpan, int)>? Changed;

    public UserInactivityListener(IMainThreadDispatcher mainThreadDispatcher)
    {
        _mainThreadDispatcher = mainThreadDispatcher ?? throw new ArgumentNullException(nameof(mainThreadDispatcher));

        _hookProc = HookProc;
    }

    public Task StartAsync() => _mainThreadDispatcher.DispatchAsync(() =>
    {
        CreateHandle(new CreateParams
        {
            Caption = "LenovoLegionToolkit_UserInactivityTimerWindow",
            Parent = new IntPtr(-3)
        });

        _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _hookProc, HINSTANCE.Null, 0);
        _mouseHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_MOUSE_LL, _hookProc, HINSTANCE.Null, 0);

        _timer = new Timer(TimerCallback, null, _timerResolution, _timerResolution);
        _tickCount = 0;

        return Task.CompletedTask;
    });

    public Task StopAsync() => _mainThreadDispatcher.DispatchAsync(() =>
    {
        _timer?.Dispose();
        _timer = null;

        PInvoke.UnhookWindowsHookEx(_kbHook);
        PInvoke.UnhookWindowsHookEx(_mouseHook);

        _kbHook = HHOOK.Null;
        _mouseHook = HHOOK.Null;

        ReleaseHandle();

        return Task.CompletedTask;
    });

    private LRESULT HookProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        lock (_lock)
        {
            _timer?.Change(_timerResolution, _timerResolution);

            if (_tickCount > 0)
            {
                _tickCount = 0;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"User became active.");

                Changed?.Invoke(this, (_timerResolution, 0));
            }
        }

        return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);
    }

    private void TimerCallback(object? state)
    {
        lock (_lock)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"User is not active [time={_timerResolution * _tickCount}]");

            _tickCount++;

            Changed?.Invoke(this, (_timerResolution, 0));
        }
    }
}