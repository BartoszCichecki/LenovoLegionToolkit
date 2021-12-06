using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class PowerPlan
    {
        public string InstanceID { get; }
        public string Name { get; }
        public bool IsActive { get; }
        public string Guid => InstanceID.Split("\\").Last().Replace("{", "").Replace("}", "");

        public PowerPlan(string instanceID, string name, bool isActive)
        {
            InstanceID = instanceID;
            Name = name;
            IsActive = isActive;
        }
    }

    public static class Power
    {
        public static void Restart() => CMD.Run("shutdown", "/r /t 0");

        public static PowerPlan[] GetPowerPlans() => WMI.Read("root\\CIMV2\\power", "SELECT * FROM Win32_PowerPlan", Create).ToArray();

        public static void ActivatePowerPlan(PowerModeState powerModeState, bool alwaysActivateDefaults = false)
        {
            var powerPlanId = Settings.Instance.PowerPlans.GetValueOrDefault(powerModeState);
            var isDefault = false;

            if (powerPlanId == null)
            {
                powerPlanId = GetDefaultPowerPlanId(powerModeState);
                isDefault = true;
            }

            if (!ShouldActivate(alwaysActivateDefaults, isDefault))
                return;

            var powerPlan = GetPowerPlans().FirstOrDefault(pp => pp.InstanceID.Contains(powerPlanId));
            if (powerPlan == null || powerPlan.IsActive)
                return;

            CMD.Run("powercfg", $"/s {powerPlan.Guid}");
        }

        private static bool ShouldActivate(bool alwaysActivateDefaults, bool isDefault)
        {
            if (isDefault)
            {
                if (alwaysActivateDefaults)
                    return true;
                if (Vantage.Status == VantageStatus.NotFound || Vantage.Status == VantageStatus.Disabled)
                    return true;
            }

            if (!isDefault)
            {
                if (Vantage.Status == VantageStatus.NotFound || Vantage.Status == VantageStatus.Disabled)
                    return true;
            }

            return false;
        }

        private static PowerPlan Create(PropertyDataCollection properties)
        {
            var instanceId = (string)properties["InstanceID"].Value;
            var name = (string)properties["ElementName"].Value;
            var isActive = (bool)properties["IsActive"].Value;
            return new(instanceId, name, isActive);
        }

        private static string GetDefaultPowerPlanId(PowerModeState state) => state switch
        {
            PowerModeState.Quiet => "16edbccd-dee9-4ec4-ace5-2f0b5f2a8975",
            PowerModeState.Balance => "85d583c5-cf2e-4197-80fd-3789a227a72c",
            PowerModeState.Performance => "52521609-efc9-4268-b9ba-67dea73f18b2",
            _ => throw new InvalidOperationException("Unknown state."),
        };
    }
}
