using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerStateListener : IListener<EventArgs>
    {
        private readonly RGBKeyboardBacklightController _rgbController;

        private bool _started;
        private PowerAdapterStatus? _lastState;

        public event EventHandler<EventArgs>? Changed;

        public PowerStateListener(RGBKeyboardBacklightController rgbController)
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
            var newState = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [e.Mode={e.Mode}, newState={newState}]");

            await RestoreRGBKeyboardState(e.Mode).ConfigureAwait(false);

            if (newState == _lastState)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Event skipped. [newState={newState}, lastState={_lastState}]");

                return;
            }

            _lastState = newState;

            if (e.Mode == PowerModes.Suspend)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Event skipped. [e.Mode={e.Mode}]");

                return;
            }

            Changed?.Invoke(this, EventArgs.Empty);
        }

        private async Task RestoreRGBKeyboardState(PowerModes mode)
        {
            if (mode != PowerModes.Resume)
                return;

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Setting light control owner and restoring preset...");

                await _rgbController.SetLightControlOwnerAsync(true, true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't set light control owner or current preset.", ex);
            }
        }
    }
}
