using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Features;

public class WhiteKeyboardBacklightFeature : IFeature<WhiteKeyboardBacklightState>
{
    private readonly WhiteKeyboardLenovoLightingBacklightFeature _lenovoLightingFeature;
    private readonly WhiteKeyboardDriverBacklightFeature _driverFeature;
    private readonly SpectrumKeyboardBacklightController _spectrumController;
    private readonly RGBKeyboardBacklightController _rgbController;

    private IFeature<WhiteKeyboardBacklightState>? _feature;

    public WhiteKeyboardBacklightFeature(WhiteKeyboardLenovoLightingBacklightFeature lenovoLightingFeature,
        WhiteKeyboardDriverBacklightFeature driverFeature,
        SpectrumKeyboardBacklightController spectrumController,
        RGBKeyboardBacklightController rgbController
        )
    {
        _lenovoLightingFeature = lenovoLightingFeature ?? throw new ArgumentNullException(nameof(lenovoLightingFeature));
        _driverFeature = driverFeature ?? throw new ArgumentNullException(nameof(driverFeature));
        _spectrumController = spectrumController ?? throw new ArgumentNullException(nameof(spectrumController));
        _rgbController = rgbController ?? throw new ArgumentNullException(nameof(rgbController));
    }

    public async Task<bool> IsSupportedAsync() => await GetFeatureAsync().ConfigureAwait(false) != null;

    public async Task<WhiteKeyboardBacklightState[]> GetAllStatesAsync()
    {
        var feature = await GetFeatureAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        return await feature.GetAllStatesAsync().ConfigureAwait(false);
    }

    public async Task<WhiteKeyboardBacklightState> GetStateAsync()
    {
        var feature = await GetFeatureAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        return await feature.GetStateAsync().ConfigureAwait(false);
    }

    public async Task SetStateAsync(WhiteKeyboardBacklightState state)
    {
        var feature = await GetFeatureAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        await feature.SetStateAsync(state).ConfigureAwait(false);
    }

    private async Task<IFeature<WhiteKeyboardBacklightState>?> GetFeatureAsync()
    {
        if (_feature is not null)
            return _feature;

        if (await _spectrumController.IsSupportedAsync().ConfigureAwait(false) || await _rgbController.IsSupportedAsync().ConfigureAwait(false))
            return null;

        if (await _lenovoLightingFeature.IsSupportedAsync().ConfigureAwait(false))
            return _feature = _lenovoLightingFeature;

        if (await _driverFeature.IsSupportedAsync().ConfigureAwait(false))
            return _feature = _driverFeature;

        return null;
    }
}
