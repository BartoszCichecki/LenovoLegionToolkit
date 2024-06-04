using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners;

public class DisplayConfigurationListener : IListener<DisplayConfigurationListener.ChangedEventArgs>
{
    public class ChangedEventArgs : EventArgs
    {
        public bool? HDR { get; init; }
    }

    private bool _started;

    public bool? IsHDROn { get; private set; }

    public event EventHandler<ChangedEventArgs>? Changed;

    public Task StartAsync()
    {
        if (_started)
            return Task.CompletedTask;

        IsHDROn = GetHDRStatus();

        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        _started = true;

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        _started = false;

        IsHDROn = null;

        return Task.CompletedTask;
    }

    private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received.");

        InternalDisplay.SetNeedsRefresh();

        var previousIsHDROn = IsHDROn;
        IsHDROn = GetHDRStatus();
        var changed = previousIsHDROn != IsHDROn;

        Changed?.Invoke(this, new() { HDR = changed ? IsHDROn : null });
    }

    private static bool? GetHDRStatus()
    {
        try
        {
            return Displays.Get().FirstOrDefault()?.GetAdvancedColorInfo().AdvancedColorEnabled;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get HDR status. Assuming unavailable.", ex);
            return null;
        }
    }
}
