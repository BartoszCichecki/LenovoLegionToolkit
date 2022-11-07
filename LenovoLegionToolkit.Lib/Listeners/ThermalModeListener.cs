using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class ThermalModeListener : AbstractWMIListener<ThermalModeState>
    {
        private readonly PowerModeFeature _powerModeFeature;
        private readonly PowerModeListener _powerModeListener;

        public ThermalModeListener(PowerModeFeature powerModeFeature, PowerModeListener powerModeListener)
            : base("ROOT\\WMI", "LENOVO_GAMEZONE_THERMAL_MODE_EVENT")
        {
            _powerModeFeature = powerModeFeature;
            _powerModeListener = powerModeListener;
        }

        protected override ThermalModeState GetValue(PropertyDataCollection properties) => ThermalModeState.IrrelevantAndBuggy;

        protected override async Task OnChangedAsync(ThermalModeState _)
        {
            var state = await _powerModeFeature.GetStateAsync();
            await _powerModeListener.NotifyAsync(state);
        }
    }
}