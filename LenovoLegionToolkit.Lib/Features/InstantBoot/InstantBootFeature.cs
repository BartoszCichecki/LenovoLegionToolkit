namespace LenovoLegionToolkit.Lib.Features.InstantBoot;

public class InstantBootFeature(InstantBootCapabilityFeature feature1, InstantBootFeatureFlagsFeature feature2)
    : AbstractCompositeFeature<InstantBootState>(feature1, feature2);
