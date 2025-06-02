using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.RemoteDesktop;

namespace LenovoLegionToolkit.Lib.Listeners;

public class SessionLockUnlockListener : IListener<SessionLockUnlockListener.ChangedEventArgs>
{
    public class ChangedEventArgs(bool locked) : EventArgs
    {
        public bool Locked { get; } = locked;
    }

    public event EventHandler<ChangedEventArgs>? Changed;

    public bool? IsLocked { get; private set; }

    public Task StartAsync()
    {
        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        return Task.CompletedTask;
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason != SessionSwitchReason.SessionLock && e.Reason != SessionSwitchReason.SessionUnlock)
            return;

        var flags = GetActiveConsoleSessionFlags();
        if (flags == PInvoke.WTS_SESSIONSTATE_UNKNOWN)
        {
            IsLocked = null;
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unknown error occured when getting active console session flags.");
            return;
        }
        var locked = (flags == PInvoke.WTS_SESSIONSTATE_LOCK);
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Session lock unlock state switched. [locked={locked}]");
        IsLocked = locked;
        Changed?.Invoke(this, new(locked));
    }

    private static unsafe uint GetActiveConsoleSessionFlags()
    {
        const WTS_INFO_CLASS infoClass = WTS_INFO_CLASS.WTSSessionInfoEx;

        var sessionFlags = PInvoke.WTS_SESSIONSTATE_UNKNOWN;
        var dwSessionId = PInvoke.WTSGetActiveConsoleSessionId();

        if (!PInvoke.WTSQuerySessionInformation(HANDLE.WTS_CURRENT_SERVER_HANDLE, dwSessionId, infoClass, out var ppBuffer, out var pBytesReturned))
            return sessionFlags;

        if (pBytesReturned > 0)
        {
            var info = Marshal.PtrToStructure<WTSINFOEXW>((IntPtr)ppBuffer.Value);
            if (info.Level == 1)
                sessionFlags = (uint)info.Data.WTSInfoExLevel1.SessionFlags;
        }

        PInvoke.WTSFreeMemory(ppBuffer);
        return sessionFlags;
    }
}
