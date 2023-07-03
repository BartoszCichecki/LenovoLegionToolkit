using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Features.PanelLogo;

public class PanelLogoSpectrumBacklightFeature : IFeature<PanelLogoBacklightState>
{
    private readonly SpectrumKeyboardBacklightController _controller;

    public PanelLogoSpectrumBacklightFeature(SpectrumKeyboardBacklightController controller)
    {
        _controller = controller;
    }

    public async Task<bool> IsSupportedAsync()
    {
        var isSupported = await _controller.IsSupportedAsync().ConfigureAwait(false);
        if (!isSupported)
            return false;

        var (layout, _, _) = await _controller.GetKeyboardLayoutAsync().ConfigureAwait(false);
        return layout == SpectrumLayout.Full;
    }

    public Task<PanelLogoBacklightState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<PanelLogoBacklightState>());

    public async Task<PanelLogoBacklightState> GetStateAsync() => await _controller.GetLogoStatusAsync().ConfigureAwait(false)
        ? PanelLogoBacklightState.On
        : PanelLogoBacklightState.Off;

    public Task SetStateAsync(PanelLogoBacklightState state) => _controller.SetLogoStatusAsync(state == PanelLogoBacklightState.On);
}
