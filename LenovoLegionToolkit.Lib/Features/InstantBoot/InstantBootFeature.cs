namespace LenovoLegionToolkit.Lib.Features.InstantBoot;

public class InstantBootFeature(InstantBootCapabilityFeature feature1, InstantBootFeatureFlagsFeature feature2)
    : AbstractCompositeFeature<InstantBootState, InstantBootCapabilityFeature, InstantBootFeatureFlagsFeature>(feature1,
        feature2);
