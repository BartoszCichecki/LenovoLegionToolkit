namespace LenovoLegionToolkit.Lib.Features.InstantBoot;

public class InstantBootFeature : AbstractCompositeFeature<InstantBootState, InstantBootCapabilityFeature, InstantBootFeatureFlagsFeature>
{
    public InstantBootFeature(InstantBootCapabilityFeature feature1, InstantBootFeatureFlagsFeature feature2) : base(feature1, feature2) { }
}
