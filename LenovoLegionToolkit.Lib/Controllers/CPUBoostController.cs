using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers;

public class CPUBoostModeController
{
    private const string ProcessorPowerManagementSubgroupGUID = "54533251-82be-4824-96c1-47b60b740d00";
    private const string PowerSettingGUID = "be337238-0d82-4146-a960-4f3749d470c7";

    public async Task<List<CPUBoostModeSettings>> GetSettingsAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting perfboostmode settings...");

        await EnsureAttributeVisibleAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting power plans...");

        var powerPlans = await Power.GetPowerPlansAsync().ConfigureAwait(false);
        var cpuBoostModes = await GetCPUBoostModesAsync().ConfigureAwait(false);

        var result = new List<CPUBoostModeSettings>();
        foreach (var powerPlan in powerPlans)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Getting perfboostmodes for power plan {powerPlan.Name}... [powerPlan.instanceID={powerPlan.InstanceId}]");

                var settings = await GetCPUBoostSettingsAsync(powerPlan, cpuBoostModes).ConfigureAwait(false);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Perfboostmodes settings retrieved for power plan {settings.PowerPlan.Name} [powerPlan.instanceID={settings.PowerPlan.InstanceId}, {string.Join(",", settings.CPUBoostModes.Select(cbm => $"{{{cbm.Name}:{cbm.Value}}}"))}, acSettingsValue={settings.ACSettingValue}, dcSettingValue={settings.DCSettingValue}]");

                result.Add(settings);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to load settings for {powerPlan.Name}.", ex);
            }
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Perfboostmode settings retrieved");

        return result;
    }

    public async Task SetSettingAsync(PowerPlan powerPlan, CPUBoostMode cpuBoostMode, bool isAC)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting perfboostmode to {cpuBoostMode.Name}... [powerPlan.name={powerPlan.Name}, powerPlan.instanceID={powerPlan.InstanceId}, cpuBoostMode.value={cpuBoostMode.Value}, isAC={isAC}]");

        var option = isAC ? "/SETACVALUEINDEX" : "/SETDCVALUEINDEX";
        await CMD.RunAsync("powercfg", $"{option} {powerPlan.Guid} {ProcessorPowerManagementSubgroupGUID} {PowerSettingGUID} {cpuBoostMode.Value}").ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Perfboostmode set to {cpuBoostMode.Name} [powerPlan.name={powerPlan.Name}, powerPlan.instanceID={powerPlan.InstanceId}, cpuBoostMode.value={cpuBoostMode.Value}, isAC={isAC}]");
    }

    private async Task EnsureAttributeVisibleAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Ensuring perfboostmode is visible...");

        await CMD.RunAsync("powercfg", "/ATTRIBUTES sub_processor perfboostmode -ATTRIB_HIDE").ConfigureAwait(false);

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
        var output = await CMD.RunAsync("powercfg", $"/QUERY {powerPlan.Guid} {ProcessorPowerManagementSubgroupGUID} {PowerSettingGUID}").ConfigureAwait(false);
        var outputLines = output
            .Split(Environment.NewLine)
            .Select(s => s.Trim());

        var matches = Regex.Matches(output, "0[xX][0-9a-fA-F]+");

        var acSettingValueString = matches[0].Value.Replace("0x", "").Trim();
        var dcSettingValueString = matches[1].Value.Replace("0x", "").Trim();

        var acSettingValue = int.Parse(acSettingValueString, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);
        var dcSettingValue = int.Parse(dcSettingValueString, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);

        return new CPUBoostModeSettings(powerPlan, cpuBoostModes, acSettingValue, dcSettingValue);
    }
}