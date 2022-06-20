using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerPlanListener : AbstractEventLogListener
    {
        private readonly ApplicationSettings _settings;
        private readonly PowerModeFeature _feature;

        public PowerPlanListener(ApplicationSettings settings, PowerModeFeature feature) : base("System", "*[System[Provider[@Name='Microsoft-Windows-UserModePowerService'] and EventID=12]]")
        {
            _settings = settings;
            _feature = feature;
        }

        protected override async Task OnChangedAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan changed...");

            var vantageStatus = await Vantage.GetStatusAsync().ConfigureAwait(false);
            var activateWhenVantageEnabled = _settings.Store.ActivatePowerProfilesWithVantageEnabled;
            if (vantageStatus == VantageStatus.Enabled && !activateWhenVantageEnabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring. [vantage.status={vantageStatus}, activateWhenVantageEnabled={activateWhenVantageEnabled}]");
                return;
            }

            var powerPlans = await Power.GetPowerPlansAsync().ConfigureAwait(false);
            var activePowerPlan = powerPlans.First(pp => pp.IsActive);

            var powerModes = Power.GetMatchingPowerModes(activePowerPlan.Guid);
            if (powerModes.Length != 1)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring. [matchingPowerModes={powerModes.Length}]");
                return;
            }

            var powerMode = powerModes[0];
            var currentPowerMode = await _feature.GetStateAsync().ConfigureAwait(false);
            if (powerMode == currentPowerMode)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Power mode already set.");
                return;
            }

            await _feature.SetStateAsync(powerMode).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power mode set.");
        }
    }
}
