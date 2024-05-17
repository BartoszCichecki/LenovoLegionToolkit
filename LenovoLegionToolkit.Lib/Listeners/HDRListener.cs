using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners;

public class HDRListener : IListener<HDRListener.ChangedEventArgs>
{
    public class ChangedEventArgs(HDRState message) : EventArgs
    {
        public HDRState Message { get; } = message;
    }

    private bool _started;

    public event EventHandler<ChangedEventArgs>? Changed;
    
    public bool IsHDROn { get; private set; }

    public bool IsOK { get; private set; }

    public Task StartAsync()
    {
        if (_started)
            return Task.CompletedTask;

        var display = InternalDisplay.Get();
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            IsOK = false;
        }
        else
        {
            IsHDROn = display.GetAdvancedColorInfo().AdvancedColorEnabled;
            IsOK = true;
        }
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        _started = true;

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        _started = false;

        return Task.CompletedTask;
    }
    
    private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received.");

        Task.Delay(100).ContinueWith(_ =>
        {
            var display = InternalDisplay.Get();
            if (display is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Built in display not found");

                return;
            }
            bool nowHDRState = display.GetAdvancedColorInfo().AdvancedColorEnabled;
            if (nowHDRState != IsHDROn || !IsOK)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"HDR state changed [nowHDRState={nowHDRState}]");

                IsHDROn = nowHDRState;
                Changed?.Invoke(this, new ChangedEventArgs(nowHDRState ? HDRState.On : HDRState.Off));
            }
            IsOK = true;
        });
    }
}
