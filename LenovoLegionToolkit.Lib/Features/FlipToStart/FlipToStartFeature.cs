namespace LenovoLegionToolkit.Lib.Features.FlipToStart;

public class FlipToStartFeature(FlipToStartCapabilityFeature feature1, FlipToStartUEFIFeature feature2)
    : AbstractCompositeFeature<FlipToStartState, FlipToStartCapabilityFeature, FlipToStartUEFIFeature>(feature1,
        feature2);
