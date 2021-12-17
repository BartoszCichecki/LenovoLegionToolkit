using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerModeListener : AbstractWMIListener<PowerModeState>
    {
        public PowerModeListener() : base("LENOVO_GAMEZONE_SMART_FAN_MODE_EVENT", "mode", 1) { }

        protected override void OnChanged(PowerModeState value)
        {
            Power.ActivatePowerPlan(value);
        }
    }
}
