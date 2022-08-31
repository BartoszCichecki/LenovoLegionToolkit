using System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class DisplayConfigurationListener : IListener<EventArgs>
    {
        private bool _started;

        public event EventHandler<EventArgs>? Changed;

        public void Start()
        {
            if (_started)
                return;

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            _started = true;
        }

        public void Stop()
        {
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            _started = false;
        }

        private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received.");

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
