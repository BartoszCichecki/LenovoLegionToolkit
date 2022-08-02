using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerStateListener : IListener<EventArgs>
    {
        private readonly RGBKeyboardBacklightController _rgbController;

        public event EventHandler<EventArgs>? Changed;

        public PowerStateListener(RGBKeyboardBacklightController rgbController)
        {
            _rgbController = rgbController;

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [mode={e.Mode}]");

            if (e.Mode == PowerModes.Suspend)
                return;

            await OnChangedAsync(e.Mode).ConfigureAwait(false);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private async Task OnChangedAsync(PowerModes mode)
        {
            if (mode != PowerModes.Resume)
                return;

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Setting light controll owner and restoring preset...");

                await _rgbController.SetLightControlOwnerAsync(true, true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't set light controll owner or current preset.", ex);
            }
        }
    }
}
