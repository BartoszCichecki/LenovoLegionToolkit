using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public class GodModeController : IGodModeController
{
    private readonly IGodModeController _controllerV1;
    private readonly IGodModeController _controllerV2;

    public GodModeController(GodModeControllerV1 controllerV1, GodModeControllerV2 controllerV2)
    {
        _controllerV1 = controllerV1 ?? throw new ArgumentNullException(nameof(controllerV1));
        _controllerV2 = controllerV2 ?? throw new ArgumentNullException(nameof(controllerV2));
    }

    public async Task<bool> NeedsVantageDisabledAsync()
    {
        var controller = await GetControllerAsync().ConfigureAwait(false);
        return await controller.NeedsVantageDisabledAsync().ConfigureAwait(false);
    }

    public async Task<bool> NeedsLegionZoneDisabledAsync()
    {
        var controller = await GetControllerAsync().ConfigureAwait(false);
        return await controller.NeedsLegionZoneDisabledAsync().ConfigureAwait(false);
    }

    public async Task<string?> GetActivePresetNameAsync()
    {
        var controller = await GetControllerAsync().ConfigureAwait(false);
        return await controller.GetActivePresetNameAsync().ConfigureAwait(false);
    }

    public async Task<GodModeState> GetStateAsync()
    {
        var controller = await GetControllerAsync().ConfigureAwait(false);
        return await controller.GetStateAsync().ConfigureAwait(false);
    }

    public async Task SetStateAsync(GodModeState state)
    {
        var controller = await GetControllerAsync().ConfigureAwait(false);
        await controller.SetStateAsync(state).ConfigureAwait(false);
    }

    public async Task ApplyStateAsync()
    {
        var controller = await GetControllerAsync().ConfigureAwait(false);
        await controller.ApplyStateAsync().ConfigureAwait(false);
    }

    public async Task<Dictionary<PowerModeState, GodModeDefaults>> GetDefaultsInOtherPowerModesAsync()
    {
        var controller = await GetControllerAsync().ConfigureAwait(false);
        return await controller.GetDefaultsInOtherPowerModesAsync().ConfigureAwait(false);
    }

    public async Task RestoreDefaultsInOtherPowerModeAsync(PowerModeState state)
    {
        var controller = await GetControllerAsync().ConfigureAwait(false);
        await controller.RestoreDefaultsInOtherPowerModeAsync(state).ConfigureAwait(false);
    }

    private async Task<IGodModeController> GetControllerAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);

        if (mi.Properties.SupportsGodModeV1)
            return _controllerV1;

        if (mi.Properties.SupportsGodModeV2)
            return _controllerV2;

        throw new InvalidOperationException("No supported version found.");
    }
}
