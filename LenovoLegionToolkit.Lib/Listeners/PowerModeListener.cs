using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerModeListener : AbstractWMIListener<PowerModeState>
    {
        public PowerModeListener() : base("SMART_FAN_MODE", "mode", 1) { }

        protected override void OnChanged(PowerModeState value)
        {
            PowerPlanController.SetPowerPlan(value);
        }
    }
}
