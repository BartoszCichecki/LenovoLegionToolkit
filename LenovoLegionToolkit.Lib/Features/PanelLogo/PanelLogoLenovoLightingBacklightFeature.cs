using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.PanelLogo;

public class PanelLogoLenovoLightingBacklightFeature() : AbstractLenovoLightingFeature<PanelLogoBacklightState>(3, 1, 0)
{
    public override async Task<bool> IsSupportedAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        if (mi.Properties.IsExcludedFromPanelLogoLenovoLighting)
            return false;

        return await base.IsSupportedAsync().ConfigureAwait(false);
    }

    protected override PanelLogoBacklightState FromInternal(int stateType, int _) => (PanelLogoBacklightState)stateType;

    protected override (int stateType, int level) ToInternal(PanelLogoBacklightState state) => ((int)state, 0);
}
