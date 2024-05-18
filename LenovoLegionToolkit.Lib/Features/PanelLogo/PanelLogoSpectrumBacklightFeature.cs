using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Features.PanelLogo;

public class PanelLogoSpectrumBacklightFeature(SpectrumKeyboardBacklightController controller) : IFeature<PanelLogoBacklightState>
{
    public async Task<bool> IsSupportedAsync()
    {
        var isSupported = await controller.IsSupportedAsync().ConfigureAwait(false);
        if (!isSupported)
            return false;

        var (layout, _, _) = await controller.GetKeyboardLayoutAsync().ConfigureAwait(false);
        return layout == SpectrumLayout.Full;
    }

    public Task<PanelLogoBacklightState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<PanelLogoBacklightState>());

    public async Task<PanelLogoBacklightState> GetStateAsync() => await controller.GetLogoStatusAsync().ConfigureAwait(false)
        ? PanelLogoBacklightState.On
        : PanelLogoBacklightState.Off;

    public Task SetStateAsync(PanelLogoBacklightState state) => controller.SetLogoStatusAsync(state == PanelLogoBacklightState.On);
}
