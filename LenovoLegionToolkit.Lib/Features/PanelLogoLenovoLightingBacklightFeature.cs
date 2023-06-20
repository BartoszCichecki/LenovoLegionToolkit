using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoLenovoLightingBacklightFeature : AbstractLenovoLightingFeature<PanelLogoBacklightState>
{
    private readonly (string machineType, string model)[] _excluded =
    {
        ("82JH", "15ITH6H"),
        ("82JK", "15ITH6"),
        ("82JM", "17ITH6H"),
        ("82JN", "17ITH6"),
        ("82JU", "15ACH6H"),
        ("82JW", "15ACH6"),
        ("82JY", "17ACH6H"),
        ("82K0", "17ACH6"),
        ("82K1", "15IHU6"),
        ("82K2", "15ACH6"),
        ("82NW", "15ACH6A")
    };

    public PanelLogoLenovoLightingBacklightFeature() : base(3) { }

    public override async Task<bool> IsSupportedAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        if (_excluded.Where(e => mi.MachineType.Contains(e.machineType) && mi.Model.Contains(e.model)).Any())
            return false;

        return await base.IsSupportedAsync().ConfigureAwait(false);
    }

    protected override PanelLogoBacklightState FromInternal(int value) => (PanelLogoBacklightState)value;

    protected override int ToInternal(PanelLogoBacklightState state) => (int)state;
}
