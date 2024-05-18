using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeFeature(IGPUModeGamezoneFeature feature1, IGPUModeCapabilityFeature feature2, IGPUModeFeatureFlagsFeature feature3)
    : AbstractCompositeFeature<IGPUModeState>(feature1, feature2, feature3)
{
    public bool ExperimentalGPUWorkingMode { get; set; }

    protected override async Task<IFeature<IGPUModeState>?> ResolveAsync()
    {
        if (!ExperimentalGPUWorkingMode)
            return await feature1.IsSupportedAsync().ConfigureAwait(false) ? feature1 : null;

        if (await feature2.IsSupportedAsync().ConfigureAwait(false))
            return feature2;

        if (await feature3.IsSupportedAsync().ConfigureAwait(false))
            return feature3;

        return null;

    }
}
