using System;
using System.Linq;
using System.Management;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class PowerPlan
    {
        public string InstanceID { get; }
        public string Name { get; }
        public bool IsActive { get; }

        public PowerPlan(string instanceID, string name, bool isActive)
        {
            InstanceID = instanceID;
            Name = name;
            IsActive = isActive;
        }
    }

    public static class PowerPlanController
    {
        public static PowerPlan[] GetAll() => WMI.Read("root\\CIMV2\\power", "SELECT * FROM Win32_PowerPlan", Create).ToArray();

        public static void SetPowerPlan(PowerModeState powerModeState)
        {
            if (VantageController.IsEnabled)
                return;

            if (!SettingsManager.Instance.PowerPlans.TryGetValue(powerModeState, out string powerPlanId))
                powerPlanId = GetDefaultPowerPlanId(powerModeState);

            var powerPlan = GetAll().FirstOrDefault(pp => pp.InstanceID.Contains(powerPlanId));
            if (powerPlan == null || powerPlan.IsActive)
                return;

            WMI.Invoke("root\\CIMV2\\power",
                "Win32_PowerPlan",
                "InstanceID",
                powerPlan.InstanceID,
                "Activate");
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
