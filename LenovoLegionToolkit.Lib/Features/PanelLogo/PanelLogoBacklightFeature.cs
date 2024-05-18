namespace LenovoLegionToolkit.Lib.Features.PanelLogo;

public class PanelLogoBacklightFeature(PanelLogoLenovoLightingBacklightFeature feature1, PanelLogoSpectrumBacklightFeature feature2)
    : AbstractCompositeFeature<PanelLogoBacklightState>(feature1, feature2);
