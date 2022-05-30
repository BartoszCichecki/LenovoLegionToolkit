using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class PowerModeFeature : AbstractWmiFeature<PowerModeState>
    {
        internal PowerModeFeature() : base("SmartFanMode", 1, "IsSupportSmartFan") { }

        public override async Task SetStateAsync(PowerModeState state)
        {
            await base.SetStateAsync(state).ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);
        }

        public async Task EnsureCorrectPowerPlanIsSetAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);
        }
    }
}