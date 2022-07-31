using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class CPUBoostModeController
    {
        private const string ProcessorPowerManagementSubgroupGUID = "54533251-82be-4824-96c1-47b60b740d00";
        private const string PowerSettingGUID = "be337238-0d82-4146-a960-4f3749d470c7";

        public async Task<List<CPUBoostModeSettings>> GetSettingsAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting perfboostmode settings...");

            await EnsureAttributeVisibleAsync();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting power plans...");

            var powerPlans = await Power.GetPowerPlansAsync().ConfigureAwait(false);
            var cpuBoostModes = await GetCPUBoostModesAsync().ConfigureAwait(false);

            var result = new List<CPUBoostModeSettings>();
            foreach (var powerPlan in powerPlans)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Getting perfboostmodes for power plan {powerPlan.Name}... [powerPlan.instanceID={powerPlan.InstanceID}]");

                var settings = await GetCPUBoostSettingsAsync(powerPlan, cpuBoostModes);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Perfboostmodes settings retrieved for power plan {settings.PowerPlan.Name} [powerPlan.instanceID={settings.PowerPlan.InstanceID}, {string.Join(",", settings.CPUBoostModes.Select(cbm => $"{{{cbm.Name}:{cbm.Value}}}"))}, acSettingsValue={settings.ACSettingValue}, dcSettingValue={settings.DCSettingValue}]");

                result.Add(settings);
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Perfboostmode settings retrieved");

            return result;
        }

        public async Task SetSettingAsync(PowerPlan powerPlan, CPUBoostMode cpuBoostMode, bool isAC)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting perfboostmode to {cpuBoostMode.Name}... [powerPlan.name={powerPlan.Name}, powerPlan.instanceID={powerPlan.InstanceID}, cpuBoostMode.value={cpuBoostMode.Value}, isAC={isAC}]");

            var option = isAC ? "/SETACVALUEINDEX" : "/SETDCVALUEINDEX";
            await CMD.RunAsync("powercfg", $"{option} {powerPlan.InstanceID} {ProcessorPowerManagementSubgroupGUID} {PowerSettingGUID} {cpuBoostMode.Value}");

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Perfboostmode set to {cpuBoostMode.Name} [powerPlan.name={powerPlan.Name}, powerPlan.instanceID={powerPlan.InstanceID}, cpuBoostMode.value={cpuBoostMode.Value}, isAC={isAC}]");
        }

        private async Task EnsureAttributeVisibleAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ensuring perfboostmode is visible...");

            await CMD.RunAsync("powercfg", "/ATTRIBUTES sub_processor perfboostmode -ATTRIB_HIDE");

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Perfboostmode is visible.");
        }

        private async Task<List<CPUBoostMode>> GetCPUBoostModesAsync()
        {
            var result = await WMI.ReadAsync("root\\CIMV2\\power",
                $"SELECT * FROM Win32_PowerSettingDefinitionPossibleValue WHERE InstanceID LIKE '%{PowerSettingGUID}%'",
                pdc =>
                {
                    var name = (string)pdc["ElementName"].Value;
                    var value = Convert.ToInt32(pdc["UInt32Value"].Value);
                    return new CPUBoostMode(value, name);
                }).ConfigureAwait(false);
            return result.ToList();
        }

        private async Task<CPUBoostModeSettings> GetCPUBoostSettingsAsync(PowerPlan powerPlan, List<CPUBoostMode> cpuBoostModes)
        {
            var output = await CMD.RunAsync("powercfg", $"/QUERY {powerPlan.Guid} {ProcessorPowerManagementSubgroupGUID} {PowerSettingGUID}");
            var outputLines = output
                .Split(Environment.NewLine)
                .Select(s => s.Trim());

            var acSettingValueString = outputLines.First(s => s.StartsWith("Current AC Power Setting Index")).Split(":").Last().Replace("0x", "").Trim();
            var dcSettingValueString = outputLines.First(s => s.StartsWith("Current DC Power Setting Index")).Split(":").Last().Replace("0x", "").Trim();

            var acSettingValue = int.Parse(acSettingValueString, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);
            var dcSettingValue = int.Parse(dcSettingValueString, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);

            return new CPUBoostModeSettings(powerPlan, cpuBoostModes, acSettingValue, dcSettingValue);
        }
    }
}
