using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Features.WhiteKeyboardBacklight;

public class WhiteKeyboardBacklightFeature : AbstractCompositeFeature<WhiteKeyboardBacklightState, WhiteKeyboardLenovoLightingBacklightFeature, WhiteKeyboardDriverBacklightFeature>
{
    private readonly SpectrumKeyboardBacklightController _spectrumController;
    private readonly RGBKeyboardBacklightController _rgbController;

    public WhiteKeyboardBacklightFeature(WhiteKeyboardLenovoLightingBacklightFeature feature1,
        WhiteKeyboardDriverBacklightFeature feature2,
        SpectrumKeyboardBacklightController spectrumController,
        RGBKeyboardBacklightController rgbController) : base(feature1, feature2)
    {
        _spectrumController = spectrumController ?? throw new ArgumentNullException(nameof(spectrumController));
        _rgbController = rgbController ?? throw new ArgumentNullException(nameof(rgbController));
    }

    protected override async Task<IFeature<WhiteKeyboardBacklightState>?> GetFeatureLazyAsync()
    {
        if (await _spectrumController.IsSupportedAsync().ConfigureAwait(false) || await _rgbController.IsSupportedAsync().ConfigureAwait(false))
            return null;

        return await base.GetFeatureLazyAsync().ConfigureAwait(false);
    }
}
