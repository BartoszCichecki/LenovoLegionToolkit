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
    private enum SettingID : uint
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

        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Applying Fan Table {fanTable}");

            var table = fanTable ?? FanTable.Default;
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

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Creating StepperValue {id}. [idRaw={(int)id:X}, defaultValue={defaultValue}, min={min}, max={max}, step={step}, steps={string.Join(", ", steps)}]");

            var stepperValue = new StepperValue(value, min, max, step, steps, defaultValue);
            stepperValues[id] = stepperValue;
        }

        var fanTableData = await GetFanTableDataAsync().ConfigureAwait(false);

        var preset = new GodModePreset
        {
            Name = "Default",
            CPULongTermPowerLimit = stepperValues.GetValueOrNull(SettingID.CPULongTermPowerLimit),
            CPUShortTermPowerLimit = stepperValues.GetValueOrNull(SettingID.CPUShortTermPowerLimit),
            CPUPeakPowerLimit = stepperValues.GetValueOrNull(SettingID.CPUPeakPowerLimit),
            CPUCrossLoadingPowerLimit = stepperValues.GetValueOrNull(SettingID.CPUCrossLoadingPowerLimit),
            CPUPL1Tau = stepperValues.GetValueOrNull(SettingID.CPUPL1Tau),
            APUsPPTPowerLimit = stepperValues.GetValueOrNull(SettingID.APUsPPTPowerLimit),
            CPUTemperatureLimit = stepperValues.GetValueOrNull(SettingID.CPUTemperatureLimit),
            GPUPowerBoost = stepperValues.GetValueOrNull(SettingID.GPUPowerBoost),
            GPUConfigurableTGP = stepperValues.GetValueOrNull(SettingID.GPUConfigurableTGP),
            GPUTemperatureLimit = stepperValues.GetValueOrNull(SettingID.GPUTemperatureLimit),
            GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = stepperValues.GetValueOrDefault(SettingID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline),
            FanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, FanTable.Default)
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Default state retrieved: {preset}");

        return preset;
    }

    #region Get/Set Value

    private Task<int> GetValue(SettingID id)
    {
        var idRaw = (uint)id & 0xFFFF00FF;
        return WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", idRaw } },
            pdc => Convert.ToInt32(pdc["Value"].Value));
    }

    private Task SetValue(SettingID id, StepperValue value)
    {
        var idRaw = (uint)id & 0xFFFF00FF;
        return WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "SetFeatureValue",
            new()
            {
                { "IDs", idRaw },
                { "value", value.Value },
            });
    }

    #endregion

    #region Capabilities

    private Task<IEnumerable<(SettingID Id, int DefaultValue, int Min, int Max, int Step)>> GetCapabilityDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CAPABILITY_DATA_01",
        pdc =>
        {
            var id = Convert.ToInt32(pdc["IDs"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultValue"].Value);
            var min = Convert.ToInt32(pdc["MinValue"].Value);
            var max = Convert.ToInt32(pdc["MaxValue"].Value);
            var step = Convert.ToInt32(pdc["Step"].Value);
            return ((SettingID)id, defaultValue, min, max, step);
        });

    private Task<IEnumerable<(SettingID Id, int Value)>> GetDiscreteDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_DISCRETE_DATA",
        pdc =>
        {
            var id = (SettingID)Convert.ToInt32(pdc["IDs"].Value);
            var value = Convert.ToInt32(pdc["Value"].Value);
            return (id, value);
        });

    #endregion

    #region Fan Table

    protected async Task<FanTableData[]?> GetFanTableDataAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Reading fan table data...");

        var data = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_TABLE_DATA",
            pdc =>
            {
                var mode = Convert.ToInt32(pdc["Mode"].Value);
                var fanId = Convert.ToByte(pdc["Fan_Id"].Value);
                var sensorId = Convert.ToByte(pdc["Sensor_ID"].Value);
                var fanSpeeds = (ushort[]?)pdc["FanTable_Data"].Value ?? Array.Empty<ushort>();
                var temps = (ushort[]?)pdc["SensorTable_Data"].Value ?? Array.Empty<ushort>();

                var type = (fanId, sensorId) switch
                {
                    (1, 4) => FanTableType.CPU,
                    (1, 1) => FanTableType.CPUSensor,
                    (2, 5) => FanTableType.GPU,
                    _ => FanTableType.Unknown,
                };

                return (mode, data: new FanTableData
                {
                    Type = type,
                    FanId = fanId,
                    SensorId = sensorId,
                    FanSpeeds = fanSpeeds,
                    Temps = temps
                });
            }).ConfigureAwait(false);

        var fanTableData = data
            .Where(d => d.mode == 255)
            .Select(d => d.data)
            .ToArray();

        if (fanTableData.Length != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table length: {string.Join(", ", fanTableData)}");

            return null;
        }

        if (fanTableData.Count(ftd => ftd.FanSpeeds.Length == 10) != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table fan speeds length: {string.Join(", ", fanTableData)}");

            return null;
        }

        if (fanTableData.Count(ftd => ftd.Temps.Length == 10) != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table temps length: {string.Join(", ", fanTableData)}");

            return null;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Fan table data: {string.Join(", ", fanTableData)}");

        return fanTableData;
    }

    protected Task SetFanTable(FanTable fanTable) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Set_Table",
        new() { { "FanTable", fanTable.GetBytes() } });

    #endregion

}
