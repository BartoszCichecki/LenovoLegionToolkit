namespace LenovoLegionToolkit.Lib.Features.FlipToStart;

public class FlipToStartFeature(FlipToStartCapabilityFeature feature1, FlipToStartUEFIFeature feature2)
    : AbstractCompositeFeature<FlipToStartState>(feature1, feature2);
