using System;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class DisplayConfigurationListener : IListener<EventArgs>
    {
        public event EventHandler<EventArgs>? Changed;

        public void Start()
        {
            Stop();

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        public void Stop() => SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;

        private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e) => Changed?.Invoke(this, EventArgs.Empty);
    }
}
