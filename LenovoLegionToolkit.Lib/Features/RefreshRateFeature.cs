using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using WindowsDisplayAPI;
using WindowsDisplayAPI.Native.DeviceContext;

namespace LenovoLegionToolkit.Lib.Features;

public class RefreshRateFeature : IFeature<RefreshRate>
{
    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task<RefreshRate[]> GetAllStatesAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting all refresh rates...");

        var display = InternalDisplay.Get();
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return Task.FromResult(Array.Empty<RefreshRate>());
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Built in display found: {display}");

        var currentSettings = display.CurrentSetting;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current built in display settings: {currentSettings.ToExtendedString()}");

        var result = display.GetPossibleSettings()
            .Where(dps => Match(dps, currentSettings))
            .Select(dps => dps.Frequency)
            .Distinct()
            .OrderBy(freq => freq)
            .Select(freq => new RefreshRate(freq))
            .ToArray();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Possible refresh rates are {string.Join(", ", result)}");

        return Task.FromResult(result);
    }

    public Task<RefreshRate> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting current refresh rate...");

        var display = InternalDisplay.Get();
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");

            return Task.FromResult(default(RefreshRate));
        }

        var currentSettings = display.CurrentSetting;
        var result = new RefreshRate(currentSettings.Frequency);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current refresh rate is {result} [currentSettings={currentSettings.ToExtendedString()}]");

        return Task.FromResult(result);
    }

    public Task SetStateAsync(RefreshRate state)
    {
        var display = InternalDisplay.Get();
        if (display is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display not found");
            throw new InvalidOperationException("Built in display not found");
        }

        var currentSettings = display.CurrentSetting;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current built in display settings: {currentSettings.ToExtendedString()}");

        if (currentSettings.Frequency == state.Frequency)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Frequency already set to {state.Frequency}");

            return Task.CompletedTask;
        }

        var possibleSettings = display.GetPossibleSettings();
        var newSettings = possibleSettings
            .Where(dps => Match(dps, currentSettings))
            .Where(dps => dps.Frequency == state.Frequency)
            .Select(dps => new DisplaySetting(dps, currentSettings.Position, currentSettings.Orientation, DisplayFixedOutput.Default))
            .FirstOrDefault();

        if (newSettings is not null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting display to {newSettings.ToExtendedString()}...");

            display.SetSettings(newSettings, true);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Display set to {newSettings.ToExtendedString()}");
        }
        else
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Could not find matching settings for frequency {state}");
        }

        return Task.CompletedTask;
    }

    private static bool Match(DisplayPossibleSetting dps, DisplaySetting ds)
    {
        if (dps.IsTooSmall())
            return false;

        var result = true;
        result &= dps.Resolution == ds.Resolution;
        result &= dps.ColorDepth == ds.ColorDepth;
        result &= dps.IsInterlaced == ds.IsInterlaced;
        return result;
    }
}