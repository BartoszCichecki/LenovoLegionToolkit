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
    private const string PROCESSOR_POWER_MANAGEMENT_SUBGROUP_GUID = "54533251-82be-4824-96c1-47b60b740d00";
    private const string POWER_SETTING_GUID = "be337238-0d82-4146-a960-4f3749d470c7";

    private readonly PowerPlanController _powerPlanController;

    public CPUBoostModeController(PowerPlanController powerPlanController)
    {
        _powerPlanController = powerPlanController ?? throw new ArgumentNullException(nameof(powerPlanController));
    }

    public async Task<List<CPUBoostModeSettings>> GetSettingsAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting perfboostmode settings...");

        await EnsureAttributeVisibleAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting power plans...");

        var powerPlans = _powerPlanController.GetPowerPlans();
        var cpuBoostModes = await GetCpuBoostModesAsync().ConfigureAwait(false);

        var result = new List<CPUBoostModeSettings>();
        foreach (var powerPlan in powerPlans)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Getting perfboostmodes for power plan {powerPlan.Name}... [powerPlan.Guid={powerPlan.Guid}]");

                var settings = await GetCpuBoostSettingsAsync(powerPlan, cpuBoostModes).ConfigureAwait(false);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Perfboostmodes settings retrieved for power plan {settings.PowerPlan.Name} [powerPlan.Guid={settings.PowerPlan.Guid}, {string.Join(", ", settings.CPUBoostModes.Select(cbm => $"{{{cbm.Name}:{cbm.Value}}}"))}, acSettingsValue={settings.ACSettingValue}, dcSettingValue={settings.DCSettingValue}]");

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

    public async Task SetSettingAsync(PowerPlan powerPlan, CPUBoostMode cpuBoostMode, bool isAc)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting perfboostmode to {cpuBoostMode.Name}... [powerPlan.name={powerPlan.Name}, powerPlan.Guid={powerPlan.Guid}, cpuBoostMode.value={cpuBoostMode.Value}, isAc={isAc}]");

        var option = isAc ? "/SETACVALUEINDEX" : "/SETDCVALUEINDEX";
        await CMD.RunAsync("powercfg", $"{option} {powerPlan.Guid} {PROCESSOR_POWER_MANAGEMENT_SUBGROUP_GUID} {POWER_SETTING_GUID} {cpuBoostMode.Value}").ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Perfboostmode set to {cpuBoostMode.Name} [powerPlan.name={powerPlan.Name}, powerPlan.Guid={powerPlan.Guid}, cpuBoostMode.value={cpuBoostMode.Value}, isAc={isAc}]");
    }

    private async Task EnsureAttributeVisibleAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Ensuring perfboostmode is visible...");

        await CMD.RunAsync("powercfg", "/ATTRIBUTES sub_processor perfboostmode -ATTRIB_HIDE").ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Perfboostmode is visible.");
    }

    private static async Task<List<CPUBoostMode>> GetCpuBoostModesAsync()
    {
        var result = await WMI.ReadAsync("root\\CIMV2\\power",
            $"SELECT * FROM Win32_PowerSettingDefinitionPossibleValue WHERE InstanceID LIKE '%{POWER_SETTING_GUID}%'",
            pdc =>
            {
                var name = (string)pdc["ElementName"].Value;
                var value = Convert.ToInt32(pdc["UInt32Value"].Value);
                return new CPUBoostMode(value, name);
            }).ConfigureAwait(false);
        return result.ToList();
    }

    private static async Task<CPUBoostModeSettings> GetCpuBoostSettingsAsync(PowerPlan powerPlan, List<CPUBoostMode> cpuBoostModes)
    {
        var (_, output) = await CMD.RunAsync("powercfg", $"/QUERY {powerPlan.Guid} {PROCESSOR_POWER_MANAGEMENT_SUBGROUP_GUID} {POWER_SETTING_GUID}").ConfigureAwait(false);

        var matches = Regex.Matches(output, "0[xX][0-9a-fA-F]+");

        var acSettingValueString = matches[0].Value.Replace("0x", "").Trim();
        var dcSettingValueString = matches[1].Value.Replace("0x", "").Trim();

        var acSettingValue = int.Parse(acSettingValueString, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);
        var dcSettingValue = int.Parse(dcSettingValueString, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);

        return new CPUBoostModeSettings(powerPlan, cpuBoostModes, acSettingValue, dcSettingValue);
    }
}