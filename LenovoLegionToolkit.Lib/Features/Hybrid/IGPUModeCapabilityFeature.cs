using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeCapabilityFeature : IFeature<IGPUModeState>
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi.Features is { Source: MachineInformation.FeatureData.SourceType.CapabilityData, IGPUMode: true };
        }
        catch
        {
            return false;
        }
    }

    public Task<IGPUModeState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<IGPUModeState>());

    public async Task<IGPUModeState> GetStateAsync()
    {
        var value = await GetFeatureValueAsync(CapabilityID.IGPUModeSupport).ConfigureAwait(false);
        var result = (IGPUModeState)value;
        return result;
    }

    public async Task SetStateAsync(IGPUModeState state)
    {
        await SetFeatureValueAsync(CapabilityID.IGPUModeSupport, (int)state).ConfigureAwait(false);
        if (await GetFeatureValueAsync(CapabilityID.IGPUModeChangeStatus).ConfigureAwait(false) == 0)
            throw new IGPUModeChangeException(state);
    }

    private static Task<int> GetFeatureValueAsync(CapabilityID id) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", id } },
            pdc => Convert.ToInt32(pdc["Value"].Value));

    private static Task SetFeatureValueAsync(CapabilityID id, int value) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "SetFeatureValue",
            new()
            {
                { "IDs", id },
                { "value", value }
            });
}
