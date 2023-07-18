using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeFeature : AbstractCompositeFeature<IGPUModeState, IGPUModeGamezoneFeature, IGPUModeCapabilityFeature, IGPUModeFeatureFlagsFeature>
{
    public bool ExperimentalGPUWorkingMode { get; set; }

    public IGPUModeFeature(IGPUModeGamezoneFeature feature1, IGPUModeCapabilityFeature feature2, IGPUModeFeatureFlagsFeature feature3) : base(feature1, feature2, feature3) { }

    protected override async Task<IFeature<IGPUModeState>?> GetFeatureLazyAsync()
    {
        if (ExperimentalGPUWorkingMode)
        {
            if (await Feature2.IsSupportedAsync().ConfigureAwait(false))
                return Feature2;

            if (await Feature3.IsSupportedAsync().ConfigureAwait(false))
                return Feature3;

            return null;
        }

        return await Feature1.IsSupportedAsync().ConfigureAwait(false) ? Feature1 : null;
    }
}
