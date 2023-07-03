namespace LenovoLegionToolkit.Lib.Features.PanelLogo;

public class PanelLogoBacklightFeature : AbstractCompositeFeature<PanelLogoBacklightState, PanelLogoLenovoLightingBacklightFeature, PanelLogoSpectrumBacklightFeature>
{
    public PanelLogoBacklightFeature(PanelLogoLenovoLightingBacklightFeature feature1, PanelLogoSpectrumBacklightFeature feature2) : base(feature1, feature2) { }
}
