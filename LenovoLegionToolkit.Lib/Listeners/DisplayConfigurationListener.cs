using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners;

public class DisplayConfigurationListener : IListener<EventArgs>
{
    private bool _started;

    public event EventHandler<EventArgs>? Changed;

    public Task StartAsync()
    {
        if (_started)
            return Task.CompletedTask;

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

        InternalDisplay.SetNeedsRefresh();

        Changed?.Invoke(this, EventArgs.Empty);
    }
}