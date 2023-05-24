using System.Collections.Generic;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public interface IGodModeController
{
    Task<bool> NeedsVantageDisabledAsync();
    Task<bool> NeedsLegionZoneDisabledAsync();
    Task<string?> GetActivePresetNameAsync();
    Task<GodModeState> GetStateAsync();
    Task SetStateAsync(GodModeState state);
    Task ApplyStateAsync();
    Task<FanTable> GetDefaultFanTableAsync();
    Task<FanTable> GetMinimumFanTableAsync();
    Task<Dictionary<PowerModeState, GodModeDefaults>> GetDefaultsInOtherPowerModesAsync();
    Task RestoreDefaultsInOtherPowerModeAsync(PowerModeState state);
}
