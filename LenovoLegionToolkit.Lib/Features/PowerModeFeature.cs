using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class PowerModeFeature : AbstractWmiFeature<PowerModeState>
    {
        public PowerModeFeature() : base("SmartFanMode", 1, "IsSupportSmartFan") { }

        public override async Task SetStateAsync(PowerModeState state)
        {
            await base.SetStateAsync(state);
            Power.ActivatePowerPlan(state, true);
        }

        public async Task EnsureCorrectPowerPlanIsSetAsync()
        {
            var state = await GetStateAsync();
            Power.ActivatePowerPlan(state, true);
        }
    }
}