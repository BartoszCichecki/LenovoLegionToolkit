using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class CPUBoostSettings
    {
        public PowerPlan PowerPlan { get; }
        public List<CPUBoostMode> CPUBoostModes { get; }
        public int ACSettingValue { get; }
        public int DCSettingValue { get; }

        public CPUBoostSettings(PowerPlan powerPlan, List<CPUBoostMode> cpuBoostModes, int acSettingValue, int dcSettingValue)
        {
            PowerPlan = powerPlan;
            CPUBoostModes = cpuBoostModes;
            ACSettingValue = acSettingValue;
            DCSettingValue = dcSettingValue;
        }
    }

    public static class CPUBoost
    {
        private const string ProcessorPowerManagementSubgroupGUID = "54533251-82be-4824-96c1-47b60b740d00";
        private const string PowerSettingGUID = "be337238-0d82-4146-a960-4f3749d470c7";

        private static readonly Regex guidRegex = new(@"(?im)[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?");
        private static readonly Regex nameRegex = new(@"(?im)\((.*)\)");
        private static readonly Regex activeRegex = new(@"(?im)\*$");

        public static List<CPUBoostSettings> GetSettings()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting perfboostmode settings...");

            EnsureAttributeVisible();


            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting power plans...");

            var powerPlans = GetPowerPlans();

            var result = new List<CPUBoostSettings>();
            foreach (var powerPlan in powerPlans)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Getting perfboostmodes for power plan {powerPlan.Name}... [powerPlan.instanceID={powerPlan.InstanceID}]");

                var settings = GetCPUBoostSettings(powerPlan);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Perfboostmodes settings retrieved for power plan {settings.PowerPlan.Name} [powerPlan.instanceID={settings.PowerPlan.InstanceID}, {string.Join(",", settings.CPUBoostModes.Select(cbm => $"{{{cbm.Name}:{cbm.Value}}}"))}, acSettingsValue={settings.ACSettingValue}, dcSettingValue={settings.DCSettingValue}]");

                result.Add(settings);
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Perfboostmode settings retrieved");

            return result;
        }

        public static void SetSetting(PowerPlan powerPlan, CPUBoostMode cpuBoostMode, bool isAC)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting perfboostmode to {cpuBoostMode.Name}... [powerPlan.name={powerPlan.Name}, powerPlan.instanceID={powerPlan.InstanceID}, cpuBoostMode.value={cpuBoostMode.Value}, isAC={isAC}]");

            var option = isAC ? "/SETACVALUEINDEX" : "/SETDCVALUEINDEX";
            CMD.Run("powercfg", $"{option} {powerPlan.InstanceID} {ProcessorPowerManagementSubgroupGUID} {PowerSettingGUID} {cpuBoostMode.Value}");

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Perfboostmode set to {cpuBoostMode.Name} [powerPlan.name={powerPlan.Name}, powerPlan.instanceID={powerPlan.InstanceID}, cpuBoostMode.value={cpuBoostMode.Value}, isAC={isAC}]");
        }

        private static void EnsureAttributeVisible()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ensuring perfboostmode is visible...");

            CMD.Run("powercfg", "/ATTRIBUTES sub_processor perfboostmode -ATTRIB_HIDE");

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Perfboostmode is visible.");
        }

        private static List<PowerPlan> GetPowerPlans()
        {
            var output = CMD.Run("powercfg", "/LIST");
            var outputLines = output
                .Split(Environment.NewLine)
                .Where(s => s.StartsWith("Power Scheme GUID", StringComparison.InvariantCultureIgnoreCase));

            var result = new List<PowerPlan>();

            foreach (var line in outputLines)
            {
                var guid = guidRegex.Match(line).Groups[0].Value;
                var name = nameRegex.Match(line).Groups[1].Value;
                var active = activeRegex.Match(line).Success;

                if (string.IsNullOrWhiteSpace(guid) || string.IsNullOrWhiteSpace(name)) { continue; }

                result.Add(new PowerPlan(guid, name, active));
            }

            return result.OrderBy(pp => pp.Name).ToList();
        }

        private static CPUBoostSettings GetCPUBoostSettings(PowerPlan powerPlan)
        {
            var output = CMD.Run("powercfg", $"/QUERY {powerPlan.InstanceID} {ProcessorPowerManagementSubgroupGUID} {PowerSettingGUID}");
            var outputLines = output
                .Split(Environment.NewLine)
                .Select(s => s.Trim());

            var possibleSettingLinesGroupped = outputLines
                .Where(s => s.StartsWith("Possible Setting", StringComparison.InvariantCultureIgnoreCase))
                .Split(2);

            var cpuBoostModes = new List<CPUBoostMode>();
            foreach (var possibleSettingLinesGroup in possibleSettingLinesGroupped)
            {
                var indexString = possibleSettingLinesGroup.ElementAt(0).Split(":").Last().Trim();
                var index = int.Parse(indexString);
                var name = possibleSettingLinesGroup.ElementAt(1).Split(":").Last().Trim();
                cpuBoostModes.Add(new CPUBoostMode(index, name));
            }

            var acSettingValueString = outputLines.First(s => s.StartsWith("Current AC Power Setting Index")).Split(":").Last().Replace("0x", "").Trim();
            var dcSettingValueString = outputLines.First(s => s.StartsWith("Current DC Power Setting Index")).Split(":").Last().Replace("0x", "").Trim();

            var acSettingValue = int.Parse(acSettingValueString, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);
            var dcSettingValue = int.Parse(dcSettingValueString, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);

            return new CPUBoostSettings(powerPlan, cpuBoostModes, acSettingValue, dcSettingValue);
        }
    }
}
