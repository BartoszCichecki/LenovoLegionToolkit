using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public class GodModeController
{
    private readonly GodModeControllerV1 _controllerV1;
    private readonly GodModeControllerV2 _controllerV2;

    public GodModeController(GodModeControllerV1 controllerV1, GodModeControllerV2 controllerV2)
    {
        _controllerV1 = controllerV1 ?? throw new ArgumentNullException(nameof(controllerV1));
        _controllerV2 = controllerV2 ?? throw new ArgumentNullException(nameof(controllerV2));
    }

    public async Task<GodModeState> GetStateAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);

        if (mi.Properties.SupportsGodModeV1)
            return await _controllerV1.GetStateAsync().ConfigureAwait(false);

        if (mi.Properties.SupportsGodModeV2)
            return await _controllerV2.GetStateAsync().ConfigureAwait(false);

        throw new InvalidOperationException("No supported version found.");
    }

    public async Task SetStateAsync(GodModeState state)
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);

        if (mi.Properties.SupportsGodModeV1)
        {
            await _controllerV1.SetStateAsync(state).ConfigureAwait(false);
            return;
        }

        if (mi.Properties.SupportsGodModeV2)
        {
            await _controllerV2.SetStateAsync(state).ConfigureAwait(false);
            return;
        }

        throw new InvalidOperationException("No supported version found.");
    }

    public async Task ApplyStateAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);

        if (mi.Properties.SupportsGodModeV1)
        {
            await _controllerV1.ApplyStateAsync().ConfigureAwait(false);
            return;
        }

        if (mi.Properties.SupportsGodModeV2)
        {
            await _controllerV2.ApplyStateAsync().ConfigureAwait(false);
            return;
        }

        throw new InvalidOperationException("No supported version found.");
    }
}
