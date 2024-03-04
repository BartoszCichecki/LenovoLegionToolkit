using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Features.WhiteKeyboardBacklight;

public class WhiteKeyboardBacklightFeature(
    WhiteKeyboardLenovoLightingBacklightFeature feature1,
    WhiteKeyboardDriverBacklightFeature feature2,
    SpectrumKeyboardBacklightController spectrumController,
    RGBKeyboardBacklightController rgbController)
    : AbstractCompositeFeature<WhiteKeyboardBacklightState, WhiteKeyboardLenovoLightingBacklightFeature, WhiteKeyboardDriverBacklightFeature>(feature1, feature2)
{
    protected override async Task<IFeature<WhiteKeyboardBacklightState>?> GetFeatureLazyAsync()
    {
        if (await spectrumController.IsSupportedAsync().ConfigureAwait(false) || await rgbController.IsSupportedAsync().ConfigureAwait(false))
            return null;

        return await base.GetFeatureLazyAsync().ConfigureAwait(false);
    }
}
