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
    public GodModeControllerV2(GodModeSettings settings, Vantage vantage, LegionZone legionZone) : base(settings, vantage, legionZone) { }

    public override Task<bool> NeedsVantageDisabledAsync() => Task.FromResult(true);
    public override Task<bool> NeedsLegionZoneDisabledAsync() => Task.FromResult(true);

    public override async Task ApplyStateAsync()
    {
        if (await Vantage.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
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

        var settings = new Dictionary<TuneId, StepperValue?>
        {
            { TuneId.CPULongTermPowerLimit, preset.CPULongTermPowerLimit },
            { TuneId.CPUShortTermPowerLimit, preset.CPUShortTermPowerLimit },
            { TuneId.CPUPeakPowerLimit, preset.CPUPeakPowerLimit },
            { TuneId.CPUCrossLoadingPowerLimit, preset.CPUCrossLoadingPowerLimit },
            { TuneId.CPUPL1Tau, preset.CPUPL1Tau },
            { TuneId.APUsPPTPowerLimit, preset.APUsPPTPowerLimit },
            { TuneId.CPUTemperatureLimit, preset.CPUTemperatureLimit },
            { TuneId.GPUPowerBoost, preset.GPUPowerBoost },
            { TuneId.GPUConfigurableTGP, preset.GPUConfigurableTGP },
            { TuneId.GPUTemperatureLimit, preset.GPUTemperatureLimit },
            { TuneId.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline, preset.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline },
        };

        var fanTable = preset.FanTable ?? FanTable.Default;
        var fanFullSpeed = preset.FanFullSpeed ?? false;

        foreach (var (id, value) in settings)
        {
            if (!value.HasValue)
                continue;

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying {id}: {value}...");

                await SetValue(id, value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to apply {id}. [value={value}]", ex);
                throw;
            }
        }

        if (fanFullSpeed)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying Fan Full Speed {fanFullSpeed}...");

                await SetFanFullSpeedAsync(fanFullSpeed).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=fanFullSpeed]", ex);
                throw;
            }
        }
        else
        {

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Making sure Fan Full Speed is false...");

                await SetFanFullSpeedAsync(false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=fanFullSpeed]", ex);
                throw;
            }

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying Fan Table {fanTable}...");

                if (!fanTable.IsValid())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Fan table invalid, replacing with default...");

                    fanTable = FanTable.Default;
                }

                await SetFanTable(fanTable).ConfigureAwait(false);
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
        var allCapabilityData = (await GetCapabilityDataAsync().ConfigureAwait(false)).ToArray();

        var capabilityData = allCapabilityData
            .Where(d => Enum.IsDefined(d.Id))
            .ToArray();

        var discreteData = (await GetDiscreteDataAsync().ConfigureAwait(false))
            .Where(d => Enum.IsDefined(d.Id))
            .GroupBy(d => d.Id, d => d.Value, (id, values) => (id, values))
            .ToDictionary(d => d.id, d => d.values.ToArray());

        var stepperValues = new Dictionary<TuneId, StepperValue>();

        foreach (var c in capabilityData)
        {
            var value = await GetValue(c.Id).OrNull().ConfigureAwait(false) ?? c.DefaultValue;
            var steps = discreteData.GetValueOrDefault(c.Id) ?? Array.Empty<int>();

            if (c.Step == 0 && steps.Length < 1)
                continue;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Creating StepperValue {c.Id}... [idRaw={(int)c.Id:X}, defaultValue={c.DefaultValue}, min={c.Min}, max={c.Max}, step={c.Step}, steps={string.Join(", ", steps)}]");

            var stepperValue = new StepperValue(value, c.Min, c.Max, c.Step, steps, c.DefaultValue);
            stepperValues[c.Id] = stepperValue;
        }

        var fanTableData = await GetFanTableDataAsync().ConfigureAwait(false);

        var preset = new GodModePreset
        {
            Name = "Default",
            CPULongTermPowerLimit = stepperValues.GetValueOrNull(TuneId.CPULongTermPowerLimit),
            CPUShortTermPowerLimit = stepperValues.GetValueOrNull(TuneId.CPUShortTermPowerLimit),
            CPUPeakPowerLimit = stepperValues.GetValueOrNull(TuneId.CPUPeakPowerLimit),
            CPUCrossLoadingPowerLimit = stepperValues.GetValueOrNull(TuneId.CPUCrossLoadingPowerLimit),
            CPUPL1Tau = stepperValues.GetValueOrNull(TuneId.CPUPL1Tau),
            APUsPPTPowerLimit = stepperValues.GetValueOrNull(TuneId.APUsPPTPowerLimit),
            CPUTemperatureLimit = stepperValues.GetValueOrNull(TuneId.CPUTemperatureLimit),
            GPUPowerBoost = stepperValues.GetValueOrNull(TuneId.GPUPowerBoost),
            GPUConfigurableTGP = stepperValues.GetValueOrNull(TuneId.GPUConfigurableTGP),
            GPUTemperatureLimit = stepperValues.GetValueOrNull(TuneId.GPUTemperatureLimit),
            GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = stepperValues.GetValueOrDefault(TuneId.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline),
            FanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, FanTable.Default),
            FanFullSpeed = await GetFanFullSpeedAsync().ConfigureAwait(false)
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Default state retrieved: {preset}");

        return preset;
    }

    #region Get/Set Value

    private static Task<int> GetValue(TuneId id)
    {
        var idRaw = (uint)id & 0xFFFF00FF;
        return WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", idRaw } },
            pdc => Convert.ToInt32(pdc["Value"].Value));
    }

    private static Task SetValue(TuneId id, StepperValue value)
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

    private static Task<IEnumerable<Capability>> GetCapabilityDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CAPABILITY_DATA_01",
        pdc =>
        {
            var id = Convert.ToInt32(pdc["IDs"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultValue"].Value);
            var min = Convert.ToInt32(pdc["MinValue"].Value);
            var max = Convert.ToInt32(pdc["MaxValue"].Value);
            var step = Convert.ToInt32(pdc["Step"].Value);
            return new Capability((TuneId)id, defaultValue, min, max, step);
        });

    private static Task<IEnumerable<Discrete>> GetDiscreteDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_DISCRETE_DATA",
        pdc =>
        {
            var id = (TuneId)Convert.ToInt32(pdc["IDs"].Value);
            var value = Convert.ToInt32(pdc["Value"].Value);
            return new Discrete(id, value);
        });

    #endregion

    #region Fan Table

    protected static async Task<FanTableData[]?> GetFanTableDataAsync()
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

    protected static Task SetFanTable(FanTable fanTable) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Set_Table",
        new() { { "FanTable", fanTable.GetBytes() } });

    #endregion

    #region Fan Full Speed

    protected static Task<bool> GetFanFullSpeedAsync()
    {
        const uint id = 0x04020000;
        return WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", id } },
            pdc => Convert.ToInt32(pdc["Value"].Value) == 1);
    }

    protected static Task SetFanFullSpeedAsync(bool enabled)
    {
        const uint id = 0x04020000;
        return WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "SetFeatureValue",
            new()
            {
                { "IDs", id },
                { "value", enabled ? 1 : 0 },
            });
    }

    #endregion

    #region Support types

    /*
     * Structure of ID:
     * 01 - CPU, 02 - GPU
     * xx - Index
     * FF - Custom Mode, 03 - Performance, 02 - Balance, 01 - Quiet
     * 00 - Unused
     */
    private enum TuneId : uint
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

    private readonly struct Capability
    {
        public TuneId Id { get; }
        public int DefaultValue { get; }
        public int Min { get; }
        public int Max { get; }
        public int Step { get; }

        public Capability(TuneId id, int defaultValue, int min, int max, int step)
        {
            Id = id;
            DefaultValue = defaultValue;
            Min = min;
            Max = max;
            Step = step;
        }
    }

    private readonly struct Discrete
    {
        public TuneId Id { get; }
        public int Value { get; }

        public Discrete(TuneId id, int value)
        {
            Id = id;
            Value = value;
        }
    }

    #endregion

}
