using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.CLI.Features;

public class FeatureRegistration<T>(string name, Func<T, string>? toStringConverter = null, Func<string, T>? fromStringConverter = null)
    : IFeatureRegistration where T : struct
{
    public string Name { get; } = name;

    private readonly Func<IFeature<T>> _feature = IoCContainer.Resolve<IFeature<T>>;

    public Task<bool> IsSupportedAsync() => _feature().IsSupportedAsync();

    public async Task<IEnumerable<string>> GetValuesAsync()
    {
        var feature = _feature();

        if (!await feature.IsSupportedAsync().ConfigureAwait(false))
            throw new InvalidOperationException("Feature is not supported");

        var states = await feature.GetAllStatesAsync().ConfigureAwait(false);
        return states.Select(s => toStringConverter?.Invoke(s) ?? s.ToString()?.ToLowerInvariant()).OfType<string>();
    }

    public async Task<string> GetValueAsync()
    {
        var feature = _feature();

        if (!await feature.IsSupportedAsync().ConfigureAwait(false))
            throw new InvalidOperationException("Feature is not supported");

        var state = await feature.GetStateAsync().ConfigureAwait(false);

        string result;

        if (toStringConverter is not null)
        {
            result = toStringConverter(state);
        }
        else
        {
            result = state.ToString()?.ToLowerInvariant() ?? throw new InvalidOperationException("Null return value");
        }

        return result;
    }

    public async Task SetValueAsync(string value)
    {
        var feature = _feature();

        if (!await feature.IsSupportedAsync().ConfigureAwait(false))
            throw new InvalidOperationException("Feature is not supported");

        var states = await feature.GetAllStatesAsync().ConfigureAwait(false);

        T state;

        if (fromStringConverter is not null)
        {
            state = fromStringConverter(value);
        }
        else
        {
            state = Enum.TryParse<T>(value, true, out var s)
                ? s
                : throw new InvalidOperationException("State is not supported");
        }

        if (!states.Contains(state))
            throw new InvalidOperationException("State is not supported");

        await feature.SetStateAsync(state).ConfigureAwait(false);
    }
}
