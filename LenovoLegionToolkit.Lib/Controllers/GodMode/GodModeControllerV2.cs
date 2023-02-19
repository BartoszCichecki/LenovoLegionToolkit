using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public class GodModeControllerV2 : AbstractGodModeController
{
    private enum SettingId
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
        GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = 0x0204FF00,
    }

    public GodModeControllerV2(GodModeSettings settings, LegionZone legionZone) : base(settings, legionZone) { }

    public override Task<GodModeState> GetStateAsync()
    {
        return Task.FromResult(new GodModeState());
    }

    public override Task ApplyStateAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task<GodModePreset> GetDefaultStateAsync()
    {
        IEnumerable<(SettingId id, int DefaultValue, int min, int max, int step)> capabilityData = (await GetCapabilityDataAsync().ConfigureAwait(false))
            .Where(d => Enum.IsDefined(d.Id));

        var discreteData = (await GetDiscreteDataAsync().ConfigureAwait(false))
            .Where(d => Enum.IsDefined(d.Id))
            .GroupBy(d => d.Id, d => d.Value, (id, values) => (id, values))
            .ToDictionary(d => d.id, d => d.values.ToArray());

        var stepperValues = capabilityData.Select(d =>
        {
            var id = d.id;
            var value = d.DefaultValue;
            var min = d.min;
            var max = d.max;
            var step = d.step;
            var steps = discreteData.GetValueOrDefault(d.id) ?? Array.Empty<int>();
            return (id, value: new StepperValue(value, min, max, step, steps, value));
        }).ToArray();

        var fanTableData = await GetFanTableDataAsync().ConfigureAwait(false);

        var preset = new GodModePreset
        {
            Name = "Default",
            CPULongTermPowerLimit = stepperValues.Where(d => d.id == SettingId.CPULongTermPowerLimit).Select(d => d.value).FirstOrDefault(),
            CPUShortTermPowerLimit = stepperValues.Where(d => d.id == SettingId.CPUShortTermPowerLimit).Select(d => d.value).FirstOrDefault(),
            CPUPeakPowerLimit = stepperValues.Where(d => d.id == SettingId.CPUPeakPowerLimit).Select(d => d.value).FirstOrDefault(),
            CPUCrossLoadingPowerLimit = stepperValues.Where(d => d.id == SettingId.CPUCrossLoadingPowerLimit).Select(d => d.value).FirstOrDefault(),
            CPUPL1Tau = stepperValues.Where(d => d.id == SettingId.CPUPL1Tau).Select(d => d.value).FirstOrDefault(),
            APUsPPTPowerLimit = stepperValues.Where(d => d.id == SettingId.APUsPPTPowerLimit).Select(d => d.value).FirstOrDefault(),
            CPUTemperatureLimit = stepperValues.Where(d => d.id == SettingId.CPUTemperatureLimit).Select(d => d.value).FirstOrDefault(),
            GPUPowerBoost = stepperValues.Where(d => d.id == SettingId.GPUPowerBoost).Select(d => d.value).FirstOrDefault(),
            GPUConfigurableTGP = stepperValues.Where(d => d.id == SettingId.GPUConfigurableTGP).Select(d => d.value).FirstOrDefault(),
            GPUTemperatureLimit = stepperValues.Where(d => d.id == SettingId.GPUTemperatureLimit).Select(d => d.value).FirstOrDefault(),
            GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = stepperValues.Where(d => d.id == SettingId.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline).Select(d => d.value).FirstOrDefault(),
            FanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, FanTable.Default),
            FanFullSpeed = await GetFanFullSpeedAsync().ConfigureAwait(false),
        };

        return preset;
    }

    private Task<IEnumerable<(SettingId Id, int DefaultValue, int Min, int Max, int Step)>> GetCapabilityDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CAPABILITY_DATA_01",
        pdc =>
        {
            var id = (SettingId)Convert.ToInt32(pdc["IDs"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultValue"].Value);
            var min = Convert.ToInt32(pdc["MinValue"].Value);
            var max = Convert.ToInt32(pdc["MaxValue"].Value);
            var step = Convert.ToInt32(pdc["Step"].Value);
            return (id, defaultValue, min, max, step);
        });

    private Task<IEnumerable<(SettingId Id, int Value)>> GetDiscreteDataAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_DISCRETE_DATA",
        pdc =>
        {
            var id = (SettingId)Convert.ToInt32(pdc["IDs"].Value);
            var value = Convert.ToInt32(pdc["Value"].Value);
            return (id, value);
        });
}
