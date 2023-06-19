namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoLenovoLightingBacklightFeature : AbstractLenovoLightingFeature<PanelLogoBacklightState>
{
    public PanelLogoLenovoLightingBacklightFeature() : base(3) { }

    protected override PanelLogoBacklightState FromInternal(int value) => (PanelLogoBacklightState)value;

    protected override int ToInternal(PanelLogoBacklightState state) => (int)state;
}
