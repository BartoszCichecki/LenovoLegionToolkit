namespace LenovoLegionToolkit.Lib.Features.FlipToStart;

public class FlipToStartFeature : AbstractCompositeFeature<FlipToStartState, FlipToStartCapabilityFeature, FlipToStartUEFIFeature>
{
    public FlipToStartFeature(FlipToStartCapabilityFeature feature1, FlipToStartUEFIFeature feature2) : base(feature1, feature2) { }
}
