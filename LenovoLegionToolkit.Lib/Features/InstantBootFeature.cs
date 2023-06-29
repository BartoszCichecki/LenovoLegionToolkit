using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features;

public class InstantBootFeature : IFeature<InstantBootState>
{
    private readonly InstantBootCapabilityFeature _capabilityFeature;
    private readonly InstantBootFeatureFlagsFeature _featureFlagsFeature;

    private readonly Lazy<Task<IFeature<InstantBootState>?>> _lazyAsyncFeature;

    public InstantBootFeature(InstantBootCapabilityFeature capabilityFeature, InstantBootFeatureFlagsFeature featureFlagsFeature)
    {
        _capabilityFeature = capabilityFeature ?? throw new ArgumentNullException(nameof(capabilityFeature));
        _featureFlagsFeature = featureFlagsFeature ?? throw new ArgumentNullException(nameof(featureFlagsFeature));

        _lazyAsyncFeature = new(GetFeatureLazyAsync);
    }

    public async Task<bool> IsSupportedAsync() => await _lazyAsyncFeature.Value.ConfigureAwait(false) != null;

    public async Task<InstantBootState[]> GetAllStatesAsync()
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        return await feature.GetAllStatesAsync().ConfigureAwait(false);
    }

    public async Task<InstantBootState> GetStateAsync()
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        return await feature.GetStateAsync().ConfigureAwait(false);
    }

    public async Task SetStateAsync(InstantBootState state)
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException("No supported feature found.");
        await feature.SetStateAsync(state).ConfigureAwait(false);
    }

    private async Task<IFeature<InstantBootState>?> GetFeatureLazyAsync()
    {
        if (await _capabilityFeature.IsSupportedAsync().ConfigureAwait(false))
            return _capabilityFeature;

        if (await _featureFlagsFeature.IsSupportedAsync().ConfigureAwait(false))
            return _featureFlagsFeature;

        return null;
    }
}
