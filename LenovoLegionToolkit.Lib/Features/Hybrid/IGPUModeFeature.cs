using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeFeature : AbstractCompositeFeature<IGPUModeState, IGPUModeCapabilityFeature, IGPUModeFeatureFlagsFeature, IGPUModeGamezoneFeature>
{
    public bool EnableLegacySwitching { get; set; }

    public IGPUModeFeature(IGPUModeCapabilityFeature feature1, IGPUModeFeatureFlagsFeature feature2, IGPUModeGamezoneFeature feature3) : base(feature1, feature2, feature3) { }

    protected override async Task<IFeature<IGPUModeState>?> GetFeatureLazyAsync()
    {
        if (EnableLegacySwitching)
            return await Feature3.IsSupportedAsync().ConfigureAwait(false) ? Feature3 : null;

        if (await Feature1.IsSupportedAsync().ConfigureAwait(false))
            return Feature1;

        if (await Feature2.IsSupportedAsync().ConfigureAwait(false))
            return Feature2;

        return null;
    }
}
