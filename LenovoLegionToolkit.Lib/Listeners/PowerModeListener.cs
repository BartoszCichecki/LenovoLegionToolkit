using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerModeListener : AbstractWMIListener<PowerModeState>
    {
        internal PowerModeListener() : base("LENOVO_GAMEZONE_SMART_FAN_MODE_EVENT", "mode", 1) { }

        protected override async void OnChanged(PowerModeState value)
        {
            await Power.ActivatePowerPlanAsync(value).ConfigureAwait(false);
        }
    }
}
