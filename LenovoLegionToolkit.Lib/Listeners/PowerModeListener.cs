using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerModeListener : AbstractWMIListener<PowerModeState>, INotifyingListener<PowerModeState>
    {
        public PowerModeListener() : base("ROOT\\WMI", "LENOVO_GAMEZONE_SMART_FAN_MODE_EVENT") { }

        protected override PowerModeState GetValue(PropertyDataCollection properties)
        {
            var property = properties["mode"];
            var propertyValue = Convert.ToInt32(property.Value);
            var value = (PowerModeState)(object)(propertyValue - 1);
            return value;
        }

        protected override Task OnChangedAsync(PowerModeState value) => Power.ActivatePowerPlanAsync(value);

        public async Task NotifyAsync(PowerModeState value)
        {
            await OnChangedAsync(value).ConfigureAwait(false);
            RaiseChanged(value);
        }
    }
}
