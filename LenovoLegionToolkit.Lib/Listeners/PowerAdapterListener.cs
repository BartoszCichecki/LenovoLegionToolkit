using System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerStateListener : IListener<EventArgs>
    {
        public event EventHandler<EventArgs>? Changed;

        public PowerStateListener()
        {
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [mode={e.Mode}]");

            if (e.Mode == PowerModes.Suspend)
                return;

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
