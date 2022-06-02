using System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class DisplayConfigurationListener : IListener<EventArgs>
    {
        public event EventHandler<EventArgs>? Changed;

        public DisplayConfigurationListener()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received.");

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
