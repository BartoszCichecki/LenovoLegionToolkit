using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoLenovoLightingBacklightFeature : AbstractLenovoLightingFeature<PanelLogoBacklightState>
{
    private readonly string[] _excludedMachineTypes =
    {
        "82JH", "82JK", "82JM", "82JN", "82JU", "82JW", "82JY", "82K0",
        "82K1", "82K2", "82NW", "82SA", "82SB", "82SC"
    };

    public PanelLogoLenovoLightingBacklightFeature() : base(3) { }

    public override async Task<bool> IsSupportedAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        if (_excludedMachineTypes.Contains(mi.MachineType))
            return false;

        return await base.IsSupportedAsync().ConfigureAwait(false);
    }

    protected override PanelLogoBacklightState FromInternal(int value) => (PanelLogoBacklightState)value;

    protected override int ToInternal(PanelLogoBacklightState state) => (int)state;
}
