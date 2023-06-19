using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoSpectrumBacklightFeature : IFeature<PanelLogoBacklightState>
{
    private readonly SpectrumKeyboardBacklightController _controller;

    public PanelLogoSpectrumBacklightFeature(SpectrumKeyboardBacklightController controller)
    {
        _controller = controller;
    }

    public Task<bool> IsSupportedAsync() => _controller.IsSupportedAsync();

    public Task<PanelLogoBacklightState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<PanelLogoBacklightState>());

    public async Task<PanelLogoBacklightState> GetStateAsync() => await _controller.GetLogoStatusAsync().ConfigureAwait(false)
        ? PanelLogoBacklightState.On
        : PanelLogoBacklightState.Off;

    public Task SetStateAsync(PanelLogoBacklightState state) => _controller.SetLogoStatusAsync(state == PanelLogoBacklightState.On);
}
