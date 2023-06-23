using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoLenovoLightingBacklightFeature : AbstractLenovoLightingFeature<PanelLogoBacklightState>
{
    protected override IEnumerable<(string machineType, string model)> Excluded => new[]
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

    public PanelLogoLenovoLightingBacklightFeature() : base(3, 1, 0) { }

    protected override PanelLogoBacklightState FromInternal(int stateType, int _) => (PanelLogoBacklightState)stateType;

    protected override (int stateType, int level) ToInternal(PanelLogoBacklightState state) => ((int)state, 0);
}
