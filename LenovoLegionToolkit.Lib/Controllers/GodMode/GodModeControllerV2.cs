using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public class GodModeControllerV2 : AbstractGodModeController
{
    public GodModeControllerV2(GodModeSettings settings, VantageDisabler vantageDisabler, LegionZoneDisabler legionZoneDisabler) : base(settings, vantageDisabler, legionZoneDisabler) { }

    public override Task<bool> NeedsVantageDisabledAsync() => Task.FromResult(true);
    public override Task<bool> NeedsLegionZoneDisabledAsync() => Task.FromResult(true);

    public override async Task ApplyStateAsync()
    {
        if (await VantageDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Vantage is running.");
            return;
        }

        if (await LegionZoneDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Legion Zone is running.");
            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Applying state...");

        var preset = await GetActivePresetAsync().ConfigureAwait(false);

        var settings = new Dictionary<LenovoFeatureID, StepperValue?>
        {
            { LenovoFeatureID.CPULongTermPowerLimit, preset.CPULongTermPowerLimit },
            { LenovoFeatureID.CPUShortTermPowerLimit, preset.CPUShortTermPowerLimit },
            { LenovoFeatureID.CPUPeakPowerLimit, preset.CPUPeakPowerLimit },
            { LenovoFeatureID.CPUCrossLoadingPowerLimit, preset.CPUCrossLoadingPowerLimit },
            { LenovoFeatureID.CPUPL1Tau, preset.CPUPL1Tau },
            { LenovoFeatureID.APUsPPTPowerLimit, preset.APUsPPTPowerLimit },
            { LenovoFeatureID.CPUTemperatureLimit, preset.CPUTemperatureLimit },
            { LenovoFeatureID.GPUPowerBoost, preset.GPUPowerBoost },
            { LenovoFeatureID.GPUConfigurableTGP, preset.GPUConfigurableTGP },
            { LenovoFeatureID.GPUTemperatureLimit, preset.GPUTemperatureLimit },
            { LenovoFeatureID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline, preset.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline },
        };

        var fanTable = preset.FanTable ?? await GetDefaultFanTableAsync().ConfigureAwait(false);
        var fanFullSpeed = preset.FanFullSpeed ?? false;

        foreach (var (id, value) in settings)
        {
            if (!value.HasValue)
                continue;

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying {id}: {value}...");

                await SetValueAsync(id, value.Value).ConfigureAwait(false);
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

                if (!await IsValidFanTableAsync(fanTable).ConfigureAwait(false))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Fan table invalid, replacing with default...");

                    fanTable = await GetDefaultFanTableAsync().ConfigureAwait(false);
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

    public override Task<FanTable> GetMinimumFanTableAsync()
    {
        var fanTable = new FanTable(new ushort[] { 1, 1, 1, 1, 1, 1, 1, 1, 3, 5 });
        return Task.FromResult(fanTable);
    }

    public override async Task<Dictionary<PowerModeState, GodModeDefaults>> GetDefaultsInOtherPowerModesAsync()
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting defaults in other power modes...");

            var result = new Dictionary<PowerModeState, GodModeDefaults>();

            var allCapabilityData = (await GetCapabilityDataAsync().ConfigureAwait(false)).ToArray();

            foreach (var powerMode in new[] { PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance })
            {
                var defaults = new GodModeDefaults
                {
                    CPULongTermPowerLimit = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.CPULongTermPowerLimit, powerMode),
                    CPUShortTermPowerLimit = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.CPUShortTermPowerLimit, powerMode),
                    CPUPeakPowerLimit = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.CPUPeakPowerLimit, powerMode),
                    CPUCrossLoadingPowerLimit = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.CPUCrossLoadingPowerLimit, powerMode),
                    CPUPL1Tau = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.CPUPL1Tau, powerMode),
                    APUsPPTPowerLimit = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.APUsPPTPowerLimit, powerMode),
                    CPUTemperatureLimit = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.CPUTemperatureLimit, powerMode),
                    GPUPowerBoost = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.GPUPowerBoost, powerMode),
                    GPUConfigurableTGP = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.GPUConfigurableTGP, powerMode),
                    GPUTemperatureLimit = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.GPUTemperatureLimit, powerMode),
                    GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = GetDefaultTuneIdValueInPowerMode(allCapabilityData, LenovoFeatureID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline, powerMode),
                    FanTable = await GetDefaultFanTableAsync().ConfigureAwait(false),
                    FanFullSpeed = false
                };

                result[powerMode] = defaults;
            }

            if (Log.Instance.IsTraceEnabled)
            {
                Log.Instance.Trace($"Defaults in other power modes retrieved:");
                foreach (var (powerMode, defaults) in result)
                    Log.Instance.Trace($" - {powerMode}: {defaults}");
            }

            return result;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get defaults in other power modes.", ex);

            return new Dictionary<PowerModeState, GodModeDefaults>();
        }
    }

    public override Task RestoreDefaultsInOtherPowerModeAsync(PowerModeState _) => Task.CompletedTask;

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

        var stepperValues = new Dictionary<LenovoFeatureID, StepperValue>();

        foreach (var c in capabilityData)
        {
            var value = await GetValueAsync(c.Id).ConfigureAwait(false).OrNullIfException() ?? c.DefaultValue;
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
            CPULongTermPowerLimit = stepperValues.GetValueOrNull(LenovoFeatureID.CPULongTermPowerLimit),
            CPUShortTermPowerLimit = stepperValues.GetValueOrNull(LenovoFeatureID.CPUShortTermPowerLimit),
            CPUPeakPowerLimit = stepperValues.GetValueOrNull(LenovoFeatureID.CPUPeakPowerLimit),
            CPUCrossLoadingPowerLimit = stepperValues.GetValueOrNull(LenovoFeatureID.CPUCrossLoadingPowerLimit),
            CPUPL1Tau = stepperValues.GetValueOrNull(LenovoFeatureID.CPUPL1Tau),
            APUsPPTPowerLimit = stepperValues.GetValueOrNull(LenovoFeatureID.APUsPPTPowerLimit),
            CPUTemperatureLimit = stepperValues.GetValueOrNull(LenovoFeatureID.CPUTemperatureLimit),
            GPUPowerBoost = stepperValues.GetValueOrNull(LenovoFeatureID.GPUPowerBoost),
            GPUConfigurableTGP = stepperValues.GetValueOrNull(LenovoFeatureID.GPUConfigurableTGP),
            GPUTemperatureLimit = stepperValues.GetValueOrNull(LenovoFeatureID.GPUTemperatureLimit),
            GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = stepperValues.GetValueOrNull(LenovoFeatureID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline),
            FanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, await GetDefaultFanTableAsync().ConfigureAwait(false)),
            FanFullSpeed = await GetFanFullSpeedAsync().ConfigureAwait(false)
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Default state retrieved: {preset}");

        return preset;
    }

    private static LenovoFeatureID AdjustTuneIdForPowerMode(LenovoFeatureID lenovoFeatureID, PowerModeState powerMode)
    {
        var tuneIdRaw = (uint)lenovoFeatureID & 0xFFFF00FF;
        var powerModeRaw = ((uint)powerMode + 1) << 8;
        return (LenovoFeatureID)(tuneIdRaw + powerModeRaw);
    }

    private static int? GetDefaultTuneIdValueInPowerMode(IEnumerable<Capability> capabilities, LenovoFeatureID lenovoFeatureID, PowerModeState powerMode)
    {
        var adjustTuneIdForPowerMode = AdjustTuneIdForPowerMode(lenovoFeatureID, powerMode);
        var value = capabilities
            .Where(c => c.Id == adjustTuneIdForPowerMode)
            .Select(c => c.DefaultValue)
            .DefaultIfEmpty(-1)
            .First();
        return value < 0 ? null : value;
    }

    #region Get/Set Value

    private static Task<int> GetValueAsync(LenovoFeatureID id)
    {
        var idRaw = (uint)id & 0xFFFF00FF;
        return WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", idRaw } },
            pdc => Convert.ToInt32(pdc["Value"].Value));
    }

    private static Task SetValueAsync(LenovoFeatureID id, StepperValue value)
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
            return new Capability((LenovoFeatureID)id, defaultValue, min, max, step);
        });

    private static Task<IEnumerable<Discrete>> GetDiscreteDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_DISCRETE_DATA",
        pdc =>
        {
            var id = (LenovoFeatureID)Convert.ToInt32(pdc["IDs"].Value);
            var value = Convert.ToInt32(pdc["Value"].Value);
            return new Discrete(id, value);
        });

    #endregion

    #region Fan Table

    private static async Task<FanTableData[]?> GetFanTableDataAsync(PowerModeState powerModeState = PowerModeState.GodMode)
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
            .Where(d => d.mode == (int)powerModeState + 1)
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

    private static Task SetFanTable(FanTable fanTable) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Set_Table",
        new() { { "FanTable", fanTable.GetBytes() } });

    #endregion

    #region Fan Full Speed

    private static Task<bool> GetFanFullSpeedAsync() =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", LenovoFeatureID.FanFullSpeed } },
            pdc => Convert.ToInt32(pdc["Value"].Value) == 1);

    private static Task SetFanFullSpeedAsync(bool enabled) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "SetFeatureValue",
            new()
            {
                { "IDs", LenovoFeatureID.FanFullSpeed },
                { "value", enabled ? 1 : 0 },
            });

    #endregion

    #region Support types

    private readonly struct Capability
    {
        public LenovoFeatureID Id { get; }
        public int DefaultValue { get; }
        public int Min { get; }
        public int Max { get; }
        public int Step { get; }

        public Capability(LenovoFeatureID id, int defaultValue, int min, int max, int step)
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
        public LenovoFeatureID Id { get; }
        public int Value { get; }

        public Discrete(LenovoFeatureID id, int value)
        {
            Id = id;
            Value = value;
        }
    }

    #endregion

}
