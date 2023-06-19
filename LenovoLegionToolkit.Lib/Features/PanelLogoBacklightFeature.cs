using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoBacklightFeature : IFeature<PanelLogoBacklightState>
{
    private readonly PanelLogoSpectrumBacklightFeature _spectrumFeature;
    private readonly PanelLogoLenovoLightingBacklightFeature _lenovoLightingFeature;

    public PanelLogoBacklightFeature(PanelLogoSpectrumBacklightFeature spectrumFeature, PanelLogoLenovoLightingBacklightFeature lenovoLightingFeature)
    {
        _spectrumFeature = spectrumFeature;
        _lenovoLightingFeature = lenovoLightingFeature;
    }

    public async Task<bool> IsSupportedAsync() => await GetFeatureAsync().ConfigureAwait(false) != null;

    public async Task<PanelLogoBacklightState[]> GetAllStatesAsync()
    {
        var feature = await GetFeatureAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        return await feature.GetAllStatesAsync().ConfigureAwait(false);
    }

    public async Task<PanelLogoBacklightState> GetStateAsync()
    {
        var feature = await GetFeatureAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        return await feature.GetStateAsync().ConfigureAwait(false);
    }

    public async Task SetStateAsync(PanelLogoBacklightState state)
    {
        var feature = await GetFeatureAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        await feature.SetStateAsync(state).ConfigureAwait(false);
    }

    private async Task<IFeature<PanelLogoBacklightState>?> GetFeatureAsync()
    {
        if (await _lenovoLightingFeature.IsSupportedAsync().ConfigureAwait(false))
            return _lenovoLightingFeature;

        if (await _spectrumFeature.IsSupportedAsync().ConfigureAwait(false))
            return _spectrumFeature;

        return null;
    }
}
