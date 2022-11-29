using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Features;

public class RefreshRateFeature : IFeature<RefreshRate>
{
    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public async Task<RefreshRate[]> GetAllStatesAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting all refresh rates...");

        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return Array.Empty<RefreshRate>();
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Built in display found: {display}");

        var currentSettings = display.CurrentSetting;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current built in display settings: {currentSettings}");

        var result = display.GetPossibleSettings()
            .Where(dps => Match(dps, currentSettings))
            .Select(dps => dps.Frequency)
            .Distinct()
            .OrderBy(freq => freq)
            .Select(freq => new RefreshRate(freq))
            .ToArray();

        if (result.Length == 1)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Single display mode found");

            return result;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Possible refresh rates are {string.Join(", ", result)}");

        return result;
    }

    public async Task<RefreshRate> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting current refresh rate...");

        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return default;
        }

        var currentSettings = display.CurrentSetting;
        var result = new RefreshRate(currentSettings.Frequency);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current refresh rate is {result} [currentSettings={currentSettings}]");

        return result;
    }

    public async Task SetStateAsync(RefreshRate state)
    {
        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");
            throw new InvalidOperationException("Built in display not found");
        }

        var currentSettings = display.CurrentSetting;

        if (currentSettings.Frequency == state.Frequency)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Frequency already set to {state.Frequency}");
            return;
        }

        var possibleSettings = display.GetPossibleSettings();
        var newSettings = possibleSettings
            .Where(dps => Match(dps, currentSettings))
            .Select(dps => new DisplaySetting(dps, currentSettings.Position))
            .FirstOrDefault(dps => dps.Frequency == state.Frequency);

        if (newSettings is not null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting display to {newSettings}");

            display.SetSettings(newSettings, true);
        }
        else
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Could not find matching settings for frequency {state}");
        }
    }

    private static bool Match(DisplayPossibleSetting dps, DisplaySetting ds)
    {
        var result = true;
        result &= dps.Resolution == ds.Resolution;
        result &= dps.ColorDepth == ds.ColorDepth;
        result &= dps.IsInterlaced == ds.IsInterlaced;
        return result;
    }
}