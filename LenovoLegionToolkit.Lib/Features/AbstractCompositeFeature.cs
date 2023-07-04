using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractCompositeFeature<T, T1, T2> : IFeature<T>
    where T : struct
    where T1 : IFeature<T>
    where T2 : IFeature<T>
{
    protected readonly T1 Feature1;
    protected readonly T2 Feature2;

    private readonly Lazy<Task<IFeature<T>?>> _lazyAsyncFeature;

    protected AbstractCompositeFeature(T1 feature1, T2 feature2)
    {
        Feature1 = feature1 ?? throw new ArgumentNullException(nameof(feature1));
        Feature2 = feature2 ?? throw new ArgumentNullException(nameof(feature2));

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
        if (await Feature1.IsSupportedAsync().ConfigureAwait(false))
            return Feature1;

        if (await Feature2.IsSupportedAsync().ConfigureAwait(false))
            return Feature2;

        return null;
    }
}

public abstract class AbstractCompositeFeature<T, T1, T2, T3> : IFeature<T>
    where T : struct
    where T1 : IFeature<T>
    where T2 : IFeature<T>
    where T3 : IFeature<T>
{
    protected readonly T1 Feature1;
    protected readonly T2 Feature2;
    protected readonly T3 Feature3;

    private readonly Lazy<Task<IFeature<T>?>> _lazyAsyncFeature;

    protected AbstractCompositeFeature(T1 feature1, T2 feature2, T3 feature3)
    {
        Feature1 = feature1 ?? throw new ArgumentNullException(nameof(feature1));
        Feature2 = feature2 ?? throw new ArgumentNullException(nameof(feature2));
        Feature3 = feature3 ?? throw new ArgumentNullException(nameof(feature3));

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
        if (await Feature1.IsSupportedAsync().ConfigureAwait(false))
            return Feature1;

        if (await Feature2.IsSupportedAsync().ConfigureAwait(false))
            return Feature2;

        if (await Feature3.IsSupportedAsync().ConfigureAwait(false))
            return Feature3;

        return null;
    }
}
