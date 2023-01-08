using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Features;

public class ResolutionFeature : IFeature<Resolution>
{
    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public async Task<Resolution[]> GetAllStatesAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting all resolutions...");

        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return Array.Empty<Resolution>();
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Built in display found: {display}");

        var currentSettings = display.CurrentSetting;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current built in display settings: {currentSettings}");

        var result = display.GetPossibleSettings()
            .Where(dps => Match(dps, currentSettings))
            .Select(dps => dps.Resolution)
            .Select(res => new Resolution(res))
            .Distinct()
            .OrderByDescending(res => res)
            .ToArray();

        if (result.Length == 1)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Single display mode found");

            return result;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Possible resolutions are {string.Join(", ", result)}");

        return result;
    }

    public async Task<Resolution> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting current resolution...");

        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return default;
        }

        var currentSettings = display.CurrentSetting;
        var result = new Resolution(currentSettings.Resolution);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current resolution is {result} [currentSettings={currentSettings}]");

        return result;
    }

    public async Task SetStateAsync(Resolution state)
    {
        var display = await DisplayExtensions.GetBuiltInDisplayAsync().ConfigureAwait(false);
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");
            throw new InvalidOperationException("Built in display not found");
        }

        var currentSettings = display.CurrentSetting;

        if (currentSettings.Resolution == state)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Resolution already set to {state}");
            return;
        }

        var possibleSettings = display.GetPossibleSettings();
        var newSettings = possibleSettings
            .Where(dps => Match(dps, currentSettings))
            .Select(dps => new DisplaySetting(dps, currentSettings.Position))
            .FirstOrDefault(dps => dps.Resolution == state);

        if (newSettings is not null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting display to {newSettings}");

            display.SetSettings(newSettings, true);
        }
        else
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Could not find matching settings for resolution {state}");
        }
    }

    private static bool Match(DisplayPossibleSetting dps, DisplaySetting ds)
    {
        var result = true;
        result &= dps.Frequency == ds.Frequency;
        result &= dps.ColorDepth == ds.ColorDepth;
        result &= dps.IsInterlaced == ds.IsInterlaced;
        return result;
    }
}