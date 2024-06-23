using System;
using System.Threading.Tasks;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractCompositeFeature<T>(params IFeature<T>[] features) : IFeature<T> where T : struct
{
    private readonly AsyncLock _lock = new();

    private bool _resolved;
    private IFeature<T>? _feature;

    public async Task<bool> IsSupportedAsync()
    {
        var feature = await ResolveInternalAsync().ConfigureAwait(false);
        if (feature is null)
            return false;
        return await feature.IsSupportedAsync().ConfigureAwait(false);
    }

    public async Task<T[]> GetAllStatesAsync()
    {
        var feature = await ResolveInternalAsync().ConfigureAwait(false)
                      ?? throw new InvalidOperationException($"No supported feature found [type={GetType().Name}");
        return await feature.GetAllStatesAsync().ConfigureAwait(false);
    }

    public async Task<T> GetStateAsync()
    {
        var feature = await ResolveInternalAsync().ConfigureAwait(false)
                      ?? throw new InvalidOperationException($"No supported feature found [type={GetType().Name}");
        return await feature.GetStateAsync().ConfigureAwait(false);
    }

    public async Task SetStateAsync(T state)
    {
        var feature = await ResolveInternalAsync().ConfigureAwait(false)
                      ?? throw new InvalidOperationException($"No supported feature found [type={GetType().Name}");
        await feature.SetStateAsync(state).ConfigureAwait(false);
    }

    protected virtual async Task<IFeature<T>?> ResolveAsync()
    {
        foreach (var feature in features)
        {
            if (!await feature.IsSupportedAsync().ConfigureAwait(false))
                continue;

            return feature;
        }

        return null;
    }

    private async Task<IFeature<T>?> ResolveInternalAsync()
    {
        using (await _lock.LockAsync().ConfigureAwait(false))
        {
            if (_resolved)
                return _feature;

            _feature = await ResolveAsync().ConfigureAwait(false);
            _resolved = true;
            return _feature;
        }
    }
}
