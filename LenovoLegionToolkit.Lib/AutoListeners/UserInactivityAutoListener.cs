using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Timer = System.Threading.Timer;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class UserInactivityAutoListener(IMainThreadDispatcher mainThreadDispatcher)
    : AbstractAutoListener<UserInactivityAutoListener.ChangedEventArgs>
{
    public class ChangedEventArgs(TimeSpan timerResolution, uint tickCount) : EventArgs
    {
        public TimeSpan TimerResolution { get; } = timerResolution;
        public uint TickCount { get; } = tickCount;
    }

    private class UserInactivityWindow : NativeWindow, IDisposable
    {
        private readonly Action _callback;

        private readonly HOOKPROC _hookProc;

        private HHOOK _kbHook;
        private HHOOK _mouseHook;

        public UserInactivityWindow(Action callback)
        {
            _callback = callback;

            _hookProc = HookProc;
        }

        public void Create()
        {
            CreateHandle(new CreateParams
            {
                Caption = "LenovoLegionToolkit_UserInactivityListenerWindow",
                Parent = new IntPtr(-3)
            });

            _kbHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _hookProc, HINSTANCE.Null, 0);
            _mouseHook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_MOUSE_LL, _hookProc, HINSTANCE.Null, 0);
        }

        private LRESULT HookProc(int nCode, WPARAM wParam, LPARAM lParam)
        {
            _callback();
            return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            PInvoke.UnhookWindowsHookEx(_kbHook);
            PInvoke.UnhookWindowsHookEx(_mouseHook);

            _kbHook = HHOOK.Null;
            _mouseHook = HHOOK.Null;

            ReleaseHandle();

            GC.SuppressFinalize(this);
        }
    }

    private readonly TimeSpan _timerResolution = TimeSpan.FromSeconds(10);
    private readonly object _lock = new();

    private UserInactivityWindow? _window;
    private uint _tickCount;
    private Timer? _timer;

    public TimeSpan InactivityTimeSpan => _timerResolution * _tickCount;

    protected override Task StartAsync() => mainThreadDispatcher.DispatchAsync(() =>
    {
        lock (_lock)
        {
            _timer = new Timer(TimerCallback, null, _timerResolution, _timerResolution);
            _tickCount = 0;

            var window = new UserInactivityWindow(WindowCallback);
            window.Create();
            _window = window;
        }

        return Task.CompletedTask;
    });

    protected override Task StopAsync() => mainThreadDispatcher.DispatchAsync(() =>
    {
        lock (_lock)
        {
            _timer?.Dispose();
            _timer = null;
            _tickCount = 0;

            _window?.Dispose();
            _window = null;
        }

        return Task.CompletedTask;
    });

    private void WindowCallback()
    {
        lock (_lock)
        {
            _timer?.Change(_timerResolution, _timerResolution);

            if (_tickCount < 1)
                return;

            _tickCount = 0;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"User became active.");

            RaiseChanged(new ChangedEventArgs(_timerResolution, 0));
        }
    }

    private void TimerCallback(object? state)
    {
        lock (_lock)
        {
            _tickCount++;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"User is not active [time={_timerResolution * _tickCount}]");

            RaiseChanged(new ChangedEventArgs(_timerResolution, _tickCount));
        }
    }
}
