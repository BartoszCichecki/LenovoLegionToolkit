using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public class GodModeControllerV2 : AbstractGodModeController
{
    /*
     * Structure of ID:
     * 01 - CPU, 02 - GPU
     * xx - Index
     * FF - Custom Mode, 03 - Performance, 02 - Balance, 01 - Quiet
     * 00 - Unused
     */
    private enum SettingID
    {
        CPUShortTermPowerLimit = 0x0101FF00,
        CPULongTermPowerLimit = 0x0102FF00,
        CPUPeakPowerLimit = 0x0103FF00,
        CPUTemperatureLimit = 0x0104FF00,
        APUsPPTPowerLimit = 0x0105FF00,
        CPUCrossLoadingPowerLimit = 0x0106FF00,
        CPUPL1Tau = 0x0107FF00,
        GPUPowerBoost = 0x0201FF00,
        GPUConfigurableTGP = 0x0202FF00,
        GPUTemperatureLimit = 0x0203FF00,
        GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = 0x0204FF00
    }

    private readonly Vantage _vantage;

    public GodModeControllerV2(GodModeSettings settings, Vantage vantage, LegionZone legionZone) : base(settings, legionZone)
    {
        _vantage = vantage;
    }

    public override async Task ApplyStateAsync()
    {
        if (await _vantage.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Vantage is running.");
            return;
        }

        if (await LegionZone.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Legion Zone is running.");
            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Applying state...");

        var activePresetId = Settings.Store.ActivePresetId;
        var presets = Settings.Store.Presets;

        if (!presets.ContainsKey(activePresetId))
            throw new InvalidOperationException($"Preset with ID {activePresetId} not found.");

        var preset = presets[activePresetId];

        var settings = new Dictionary<SettingID, StepperValue?>
        {
            { SettingID.CPULongTermPowerLimit, preset.CPULongTermPowerLimit },
            { SettingID.CPUShortTermPowerLimit, preset.CPUShortTermPowerLimit },
            { SettingID.CPUPeakPowerLimit, preset.CPUPeakPowerLimit },
            { SettingID.CPUCrossLoadingPowerLimit, preset.CPUCrossLoadingPowerLimit },
            { SettingID.CPUPL1Tau, preset.CPUPL1Tau },
            { SettingID.APUsPPTPowerLimit, preset.APUsPPTPowerLimit },
            { SettingID.CPUTemperatureLimit, preset.CPUTemperatureLimit },
            { SettingID.GPUPowerBoost, preset.GPUPowerBoost },
            { SettingID.GPUConfigurableTGP, preset.GPUConfigurableTGP },
            { SettingID.GPUTemperatureLimit, preset.GPUTemperatureLimit },
            { SettingID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline, preset.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline },
        };

        var fanTable = preset.FanTable;
        var maxFan = preset.FanFullSpeed ?? false;

        foreach (var (id, value) in settings)
        {
            if (!value.HasValue)
                continue;

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying {id}: {value}");

                await SetValue(id, value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to apply {id}. [value={value}]", ex);
                throw;
            }
        }

        if (fanTable is null || maxFan)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying Fan Full Speed: {maxFan}");

                await SetFanFullSpeedAsync(maxFan).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to apply Fan Full Speed. [value={maxFan}]", ex);
                throw;
            }
        }
        else
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Making sure Fan Full Speed is off...");

                await SetFanFullSpeedAsync(false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to apply Fan Full Speed. [value=false]", ex);
                throw;
            }

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying Fan Table {fanTable.Value}");

                var table = fanTable.Value;
                if (!table.IsValid())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Fan table invalid, replacing with default...");

                    table = FanTable.Default;
                }

                await SetFanTable(table).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=fanTable]", ex);
                throw;
            }
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State applied.");
    }

    protected override async Task<GodModePreset> GetDefaultStateAsync()
    {
        IEnumerable<(SettingID id, int defaultValue, int min, int max, int step)> capabilityData = (await GetCapabilityDataAsync().ConfigureAwait(false))
            .Where(d => Enum.IsDefined(d.Id))
            .ToArray();

        var discreteData = (await GetDiscreteDataAsync().ConfigureAwait(false))
            .Where(d => Enum.IsDefined(d.Id))
            .GroupBy(d => d.Id, d => d.Value, (id, values) => (id, values))
            .ToDictionary(d => d.id, d => d.values.ToArray());

        var stepperValues = new Dictionary<SettingID, StepperValue>();

        foreach (var (id, defaultValue, min, max, step) in capabilityData)
        {
            var value = await GetValue(id).OrNull().ConfigureAwait(false) ?? defaultValue;
            var steps = discreteData.GetValueOrDefault(id) ?? Array.Empty<int>();

            if (step == 0 && steps.Length < 1)
                continue;

            var stepperValue = new StepperValue(value, min, max, step, steps, defaultValue);
            stepperValues[id] = stepperValue;
        }

        var fanTableData = await GetFanTableDataAsync().ConfigureAwait(false);

        var preset = new GodModePreset
        {
            Name = "Default",
            CPULongTermPowerLimit = stepperValues.GetValueOrDefault(SettingID.CPULongTermPowerLimit),
            CPUShortTermPowerLimit = stepperValues.GetValueOrDefault(SettingID.CPUShortTermPowerLimit),
            CPUPeakPowerLimit = stepperValues.GetValueOrDefault(SettingID.CPUPeakPowerLimit),
            CPUCrossLoadingPowerLimit = stepperValues.GetValueOrDefault(SettingID.CPUCrossLoadingPowerLimit),
            CPUPL1Tau = stepperValues.GetValueOrDefault(SettingID.CPUPL1Tau),
            APUsPPTPowerLimit = stepperValues.GetValueOrDefault(SettingID.APUsPPTPowerLimit),
            CPUTemperatureLimit = stepperValues.GetValueOrDefault(SettingID.CPUTemperatureLimit),
            GPUPowerBoost = stepperValues.GetValueOrDefault(SettingID.GPUPowerBoost),
            GPUConfigurableTGP = stepperValues.GetValueOrDefault(SettingID.GPUConfigurableTGP),
            GPUTemperatureLimit = stepperValues.GetValueOrDefault(SettingID.GPUTemperatureLimit),
            GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = stepperValues.GetValueOrDefault(SettingID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline),
            FanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, FanTable.Default),
            FanFullSpeed = await GetFanFullSpeedAsync().ConfigureAwait(false),
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Default state retrieved: {preset}");

        return preset;
    }

    private Task<int> GetValue(SettingID id) => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", $"{(int)id}" } },
            pdc => Convert.ToInt32(pdc["Value"].Value));

    private Task SetValue(SettingID id, StepperValue value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "SetFeatureValue",
        new()
        {
            { "IDs", $"{(int)id}" },
            { "value", $"{value.Value}" },
        });

    private Task<IEnumerable<(SettingID Id, int DefaultValue, int Min, int Max, int Step)>> GetCapabilityDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CAPABILITY_DATA_01",
        pdc =>
        {
            var id = (SettingID)Convert.ToInt32(pdc["IDs"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultValue"].Value);
            var min = Convert.ToInt32(pdc["MinValue"].Value);
            var max = Convert.ToInt32(pdc["MaxValue"].Value);
            var step = Convert.ToInt32(pdc["Step"].Value);
            return (id, defaultValue, min, max, step);
        });

    private Task<IEnumerable<(SettingID Id, int Value)>> GetDiscreteDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_DISCRETE_DATA",
        pdc =>
        {
            var id = (SettingID)Convert.ToInt32(pdc["IDs"].Value);
            var value = Convert.ToInt32(pdc["Value"].Value);
            return (id, value);
        });
}
