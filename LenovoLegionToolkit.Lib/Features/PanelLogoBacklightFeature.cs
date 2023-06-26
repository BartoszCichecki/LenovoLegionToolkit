using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoBacklightFeature : IFeature<PanelLogoBacklightState>
{
    private readonly PanelLogoLenovoLightingBacklightFeature _lenovoLightingFeature;
    private readonly PanelLogoSpectrumBacklightFeature _spectrumFeature;

    private readonly Lazy<Task<IFeature<PanelLogoBacklightState>?>> _lazyAsyncFeature;

    public PanelLogoBacklightFeature(PanelLogoLenovoLightingBacklightFeature lenovoLightingFeature, PanelLogoSpectrumBacklightFeature spectrumFeature)
    {
        _lenovoLightingFeature = lenovoLightingFeature ?? throw new ArgumentNullException(nameof(lenovoLightingFeature));
        _spectrumFeature = spectrumFeature ?? throw new ArgumentNullException(nameof(spectrumFeature));

        _lazyAsyncFeature = new(GetFeatureLazyAsync);
    }

    public async Task<bool> IsSupportedAsync() => await _lazyAsyncFeature.Value.ConfigureAwait(false) != null;

    public async Task<PanelLogoBacklightState[]> GetAllStatesAsync()
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        return await feature.GetAllStatesAsync().ConfigureAwait(false);
    }

    public async Task<PanelLogoBacklightState> GetStateAsync()
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        return await feature.GetStateAsync().ConfigureAwait(false);
    }

    public async Task SetStateAsync(PanelLogoBacklightState state)
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        await feature.SetStateAsync(state).ConfigureAwait(false);
    }

    private async Task<IFeature<PanelLogoBacklightState>?> GetFeatureLazyAsync()
    {
        if (await _lenovoLightingFeature.IsSupportedAsync().ConfigureAwait(false))
            return _lenovoLightingFeature;

        if (await _spectrumFeature.IsSupportedAsync().ConfigureAwait(false))
            return _spectrumFeature;

        return null;
    }
}
