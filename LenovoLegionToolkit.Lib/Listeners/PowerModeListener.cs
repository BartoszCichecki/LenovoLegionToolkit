using System;
using System.ComponentModel.DataAnnotations;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerModeListener : AbstractWMIListener<PowerModeState>, INotifyingListener<PowerModeState>
    {
        private readonly AIModeController _aiModeController;
        private readonly PowerModeFeature _powerModeFeature;
        private readonly GodModeController _godModeController;

        public PowerModeListener(AIModeController aiModeController, PowerModeFeature powerModeFeature, GodModeController godModeController)
            : base("ROOT\\WMI", "LENOVO_GAMEZONE_THERMAL_MODE_EVENT")
        {
            _aiModeController = aiModeController ?? throw new ArgumentNullException(nameof(aiModeController));
            _powerModeFeature = powerModeFeature ?? throw new ArgumentNullException(nameof(powerModeFeature));
            _godModeController = godModeController ?? throw new ArgumentNullException(nameof(godModeController));
        }

        protected override PowerModeState GetValue(PropertyDataCollection properties)
        {
            var property = properties["mode"];
            var propertyValue = Convert.ToInt32(property.Value);
            var value = (PowerModeState)(object)(propertyValue - 1);
            return value;
        }

        protected override async Task OnChangedAsync(PowerModeState value)
        {
            await _aiModeController.StartStopAsync(value).ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(value).ConfigureAwait(false);

            if (value == PowerModeState.GodMode)
                await _godModeController.ApplyStateAsync().ConfigureAwait(false);
        }

        public async Task NotifyAsync(PowerModeState value)
        {
            await OnChangedAsync(value).ConfigureAwait(false);
            RaiseChanged(value);
        }

        protected override async void Handler(PropertyDataCollection properties)
        {
            var value = GetValue(properties);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [value={value}, listener={GetType().Name}]");

            // Seems we can't trust the mode received from LENOVO_GAMEZONE_THERMAL_MODE_EVENT,
            // but at least it does fire reliably and we can call GetThermalMode ourselves.
            value = await _powerModeFeature.GetActualStateAsync().ConfigureAwait(false);

            var targetState = await _powerModeFeature.GetStateAsync().ConfigureAwait(false);

            // _powerModeFeature.GetActualStateAsync() doesn't return PowerModeState.GodMode so we have to check manually.
            if (value == PowerModeState.Performance && targetState == PowerModeState.GodMode)
                value = targetState;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Patched the event. [value={value}, listener={GetType().Name}]");

            await OnChangedAsync(value).ConfigureAwait(false);
            RaiseChanged(value);
        }
    }
}
