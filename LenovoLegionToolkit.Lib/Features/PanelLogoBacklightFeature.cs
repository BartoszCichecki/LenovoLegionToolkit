namespace LenovoLegionToolkit.Lib.Features;

public class PanelLogoBacklightFeature : AbstractLenovoLightingFeature<PanelLogoBacklightState>
{
    public PanelLogoBacklightFeature() : base(3) { }

    protected override PanelLogoBacklightState FromInternal(int value) => (PanelLogoBacklightState)value;

    protected override int ToInternal(PanelLogoBacklightState state) => (int)state;
}
