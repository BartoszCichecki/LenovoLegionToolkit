using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Automation.Listeners
{
    public class PowerStateAutomationListener : IListener<EventArgs>
    {
        private readonly RGBKeyboardBacklightController _rgbController;

        private bool _started;
        private PowerAdapterStatus? _lastState;

        public event EventHandler<EventArgs>? Changed;

        public PowerStateAutomationListener(RGBKeyboardBacklightController rgbController)
        {
            _rgbController = rgbController ?? throw new ArgumentNullException(nameof(rgbController));
        }

        public async Task StartAsync()
        {
            if (_started)
                return;

            _lastState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            _started = true;
        }

        public Task StopAsync()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            _started = false;

            return Task.CompletedTask;
        }

        private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [mode={e.Mode}]");

            var newState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

            if (newState == _lastState)
                return;

            _lastState = newState;

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
