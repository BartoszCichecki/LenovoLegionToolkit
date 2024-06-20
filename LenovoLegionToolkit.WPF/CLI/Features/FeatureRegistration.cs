using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.CLI.Features;

public readonly struct FeatureRegistration<T>() : IFeatureRegistration where T : struct
{
    public string Name { get; } = typeof(T).Name.Replace("State", null).ToLowerInvariant();

    private readonly Func<IFeature<T>> _feature = IoCContainer.Resolve<IFeature<T>>;

    public Task<bool> IsSupportedAsync()
    {
        return _feature().IsSupportedAsync();
    }

    public async Task<IEnumerable<string>> GetValuesAsync()
    {
        var feature = _feature();

        if (!await feature.IsSupportedAsync().ConfigureAwait(false))
            throw new InvalidOperationException("Feature is not supported");

        var states = await feature.GetAllStatesAsync().ConfigureAwait(false);
        return states.Select(s => s.ToString()?.ToLowerInvariant()).OfType<string>();
    }

    public async Task SetValueAsync(string value)
    {
        var feature = _feature();

        if (!await feature.IsSupportedAsync().ConfigureAwait(false))
            throw new InvalidOperationException("Feature is not supported");

        var states = await feature.GetAllStatesAsync().ConfigureAwait(false);
        var state = Enum.TryParse<T>(value, true, out var s)
            ? s
            : throw new InvalidOperationException("State is not supported");

        if (!states.Contains(state))
            throw new InvalidOperationException("State is not supported");

        await feature.SetStateAsync(state).ConfigureAwait(false);
    }

    public async Task<string> GetValueAsync()
    {
        var feature = _feature();

        if (!await feature.IsSupportedAsync().ConfigureAwait(false))
            throw new InvalidOperationException("Feature is not supported");

        var state = await feature.GetStateAsync().ConfigureAwait(false);
        return state.ToString()?.ToLowerInvariant() ?? throw new InvalidOperationException("Null return value");
    }
}
