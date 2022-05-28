using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerAdapterListener
    {
        public PowerAdapterListener()
        {
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
        }
    }
}
