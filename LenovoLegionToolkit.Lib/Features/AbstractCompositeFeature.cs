using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractCompositeFeature<T, T1, T2> : IFeature<T>
    where T : struct
    where T1 : IFeature<T>
    where T2 : IFeature<T>
{
    private readonly T1 _feature1;
    private readonly T2 _feature2;

    private readonly Lazy<Task<IFeature<T>?>> _lazyAsyncFeature;

    protected AbstractCompositeFeature(T1 feature1, T2 feature2)
    {
        _feature1 = feature1 ?? throw new ArgumentNullException(nameof(feature1));
        _feature2 = feature2 ?? throw new ArgumentNullException(nameof(feature2));

        _lazyAsyncFeature = new(GetFeatureLazyAsync);
    }

    public async Task<bool> IsSupportedAsync() => await _lazyAsyncFeature.Value.ConfigureAwait(false) != null;

    public async Task<T[]> GetAllStatesAsync()
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found. [type={GetType().Name}");
        return await feature.GetAllStatesAsync().ConfigureAwait(false);
    }

    public async Task<T> GetStateAsync()
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found. [type={GetType().Name}");
        return await feature.GetStateAsync().ConfigureAwait(false);
    }

    public async Task SetStateAsync(T state)
    {
        var feature = await _lazyAsyncFeature.Value.ConfigureAwait(false) ?? throw new InvalidOperationException($"No supported feature found. [type={GetType().Name}");
        await feature.SetStateAsync(state).ConfigureAwait(false);
    }

    protected virtual async Task<IFeature<T>?> GetFeatureLazyAsync()
    {
        if (await _feature1.IsSupportedAsync().ConfigureAwait(false))
            return _feature1;

        if (await _feature2.IsSupportedAsync().ConfigureAwait(false))
            return _feature2;

        return null;
    }
}
