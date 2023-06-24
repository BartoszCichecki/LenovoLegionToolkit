using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoLenovoLightingBacklightFeature : AbstractLenovoLightingFeature<PanelLogoBacklightState>
{
    public PanelLogoLenovoLightingBacklightFeature() : base(3, 1, 0) { }

    protected override IEnumerable<(string machineType, string model)> GetExcludedModels()
    {
        using var enumerator = base.GetExcludedModels().GetEnumerator();
        while (enumerator.MoveNext())
            yield return enumerator.Current;

        // Falsely report available panel logo
        yield return ("82JH", "15ITH6H");
        yield return ("82JK", "15ITH6");
        yield return ("82JM", "17ITH6H");
        yield return ("82JN", "17ITH6");
        yield return ("82JU", "15ACH6H");
        yield return ("82JW", "15ACH6");
        yield return ("82JY", "17ACH6H");
        yield return ("82K0", "17ACH6");
        yield return ("82K1", "15IHU6");
        yield return ("82K2", "15ACH6");
        yield return ("82NW", "15ACH6A");
    }

    protected override PanelLogoBacklightState FromInternal(int stateType, int _) => (PanelLogoBacklightState)stateType;

    protected override (int stateType, int level) ToInternal(PanelLogoBacklightState state) => ((int)state, 0);
}
