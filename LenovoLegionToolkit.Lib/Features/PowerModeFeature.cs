using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class PowerModeFeature : AbstractWmiFeature<PowerModeState>
    {
        public PowerModeFeature() : base("SmartFanMode", 1, "IsSupportSmartFan") { }

        public override PowerModeState GetState()
        {
            var state = base.GetState();
            Power.ActivatePowerPlan(state);
            return state;
        }

        public override void SetState(PowerModeState state)
        {
            base.SetState(state);
            Power.ActivatePowerPlan(state, true);
        }
    }
}