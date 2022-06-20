using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System
{
    public static class Power
    {
        private static readonly Dictionary<PowerModeState, string> defaultPowerModes = new()
        {
            { PowerModeState.Quiet , "16edbccd-dee9-4ec4-ace5-2f0b5f2a8975"},
            { PowerModeState.Balance , "85d583c5-cf2e-4197-80fd-3789a227a72c"},
            { PowerModeState.Performance , "52521609-efc9-4268-b9ba-67dea73f18b2"},
        };

        private static ApplicationSettings Settings => IoCContainer.Resolve<ApplicationSettings>();

        public static bool IsPowerAdapterConnected()
        {
            if (!Native.GetSystemPowerStatus(out SystemPowerStatusEx sps))
                return true;
            return sps.ACLineStatus == ACLineStatusEx.Online;
        }

        public static async Task RestartAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Restarting...");

            await CMD.RunAsync("shutdown", "/r /t 0").ConfigureAwait(false);
        }

        public static async Task<PowerPlan[]> GetPowerPlansAsync()
        {
            var result = await WMI.ReadAsync("root\\CIMV2\\power",
                            $"SELECT * FROM Win32_PowerPlan",
                            Create).ConfigureAwait(false);
            return result.ToArray();
        }

        public static async Task ActivatePowerPlanAsync(PowerModeState powerModeState, bool alwaysActivateDefaults = false)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Activating... [powerModeState={powerModeState}, alwaysActivateDefaults={alwaysActivateDefaults}]");

            var powerPlanId = Settings.Store.PowerPlans.GetValueOrDefault(powerModeState);
            var isDefault = false;

            if (powerPlanId is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Power plan for power mode {powerModeState} was not found in settings");

                powerPlanId = GetDefaultPowerPlanId(powerModeState);
                isDefault = true;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan to be activated is {powerPlanId} [isDefault={isDefault}]");

            if (!await ShouldActivateAsync(alwaysActivateDefaults, isDefault).ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Power plan {powerPlanId} will not be activated [isDefault={isDefault}]");

                return;
            }

            var powerPlan = (await GetPowerPlansAsync()).FirstOrDefault(pp => pp.InstanceID.Contains(powerPlanId));
            if (powerPlan.Equals(default(PowerPlan)))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Power plan {powerPlanId} was not found");

                return;
            }
            if (powerPlan.IsActive)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Power plan {powerPlanId} is already active");

                return;
            }

            await CMD.RunAsync("powercfg", $"/s {powerPlan.Guid}").ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan {powerPlan.Guid} activated");
        }

        public static PowerModeState[] GetMatchingPowerModes(string powerPlanId)
        {
            var powerModes = new Dictionary<PowerModeState, string>(defaultPowerModes);

            foreach (var kv in Settings.Store.PowerPlans)
            {
                if (string.IsNullOrWhiteSpace(kv.Value))
                    continue;

                var value = kv.Value;
                value = value[(value.IndexOf('{') + 1)..];
                value = value[..value.IndexOf('}')];


                powerModes[kv.Key] = value;
            }

            return powerModes.Where(kv => kv.Value == powerPlanId)
                .Select(kv => kv.Key)
                .ToArray();
        }

        private static async Task<bool> ShouldActivateAsync(bool alwaysActivateDefaults, bool isDefault)
        {
            var activateWhenVantageEnabled = Settings.Store.ActivatePowerProfilesWithVantageEnabled;
            if (activateWhenVantageEnabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Activate power profiles with Vantage is enabled");

                return true;
            }

            if (isDefault && alwaysActivateDefaults)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Power plan is default and always active defaults is set");

                return true;
            }

            var status = await Vantage.GetStatusAsync().ConfigureAwait(false);
            if (status == VantageStatus.NotFound || status == VantageStatus.Disabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Vantage is active [status={status}]");

                return true;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Criteria for activation not met [activateWhenVantageEnabled={activateWhenVantageEnabled}, isDefault={isDefault}, alwaysActivateDefaults={alwaysActivateDefaults}, status={status}]");

            return false;
        }

        private static PowerPlan Create(PropertyDataCollection properties)
        {
            var instanceId = (string)properties["InstanceID"].Value;
            var name = (string)properties["ElementName"].Value;
            var isActive = (bool)properties["IsActive"].Value;
            return new(instanceId, name, isActive);
        }

        private static string GetDefaultPowerPlanId(PowerModeState state)
        {
            if (defaultPowerModes.TryGetValue(state, out var powerPlanId))
                return powerPlanId;

            throw new InvalidOperationException("Unknown state");
        }
    }
}
