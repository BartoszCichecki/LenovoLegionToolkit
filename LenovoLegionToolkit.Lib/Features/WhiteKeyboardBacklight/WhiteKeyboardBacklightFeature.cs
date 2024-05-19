using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Features.WhiteKeyboardBacklight;

public class WhiteKeyboardBacklightFeature(WhiteKeyboardLenovoLightingBacklightFeature feature1,
    WhiteKeyboardDriverBacklightFeature feature2,
    SpectrumKeyboardBacklightController spectrumController,
    RGBKeyboardBacklightController rgbController)
    : AbstractCompositeFeature<WhiteKeyboardBacklightState>(feature1, feature2)
{
    protected override async Task<IFeature<WhiteKeyboardBacklightState>?> ResolveAsync()
    {
        if (await spectrumController.IsSupportedAsync().ConfigureAwait(false) || await rgbController.IsSupportedAsync().ConfigureAwait(false))
            return null;

        return await base.ResolveAsync().ConfigureAwait(false);
    }
}
