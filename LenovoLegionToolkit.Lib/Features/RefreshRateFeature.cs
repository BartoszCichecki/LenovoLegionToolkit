using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Features
{
    public class RefreshRateFeature : IDynamicFeature<RefreshRate>
    {
        public async Task<RefreshRate[]> GetAllStatesAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting all refresh rates...");

            var display = await GetBuiltInDisplayAsync().ConfigureAwait(false);
            if (display == null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Built in display not found");

                throw new InvalidOperationException("Built in display not found");
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Built in display found: {display}");

            var currentSettings = display.CurrentSetting;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Current built in display settings: {currentSettings}");

            var possibleSettings = display.GetPossibleSettings();

            if (Log.Instance.IsTraceEnabled)
            {
                Log.Instance.Trace($"Found possible settings:");
                foreach (var possibleSetting in possibleSettings)
                    Log.Instance.Trace($" - {possibleSetting}");
            }

            var result = possibleSettings.Where(dps => Match(dps, currentSettings))
                .Select(dps => dps.Frequency)
                .Distinct()
                .OrderBy(freq => freq)
                .Select(freq => new RefreshRate(freq))
                .ToArray();

            if (result.Length == 1)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Single display mode found");

                throw new InvalidOperationException("Single display mode found");
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Possible refresh rates are {string.Join(", ", result)}");

            return result;
        }

        public async Task<RefreshRate> GetStateAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting current refresh rate...");

            var display = await GetBuiltInDisplayAsync().ConfigureAwait(false);
            if (display == null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Built in display not found");

                throw new InvalidOperationException("Built in display not found");
            }

            var currentSettings = display.CurrentSetting;
            var result = new RefreshRate(currentSettings.Frequency);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Current refresh rate is {result} [currentSettings={currentSettings}]");

            return result;
        }

        public async Task SetStateAsync(RefreshRate state)
        {
            var display = await GetBuiltInDisplayAsync().ConfigureAwait(false);
            if (display == null)
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

            if (newSettings != null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Settings display to {newSettings}");

                display.SetSettings(newSettings, true);
            }
            else
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Could not find matching settings for frequency {state}");
            }
        }

        private static async Task<Display?> GetBuiltInDisplayAsync()
        {
            var displays = Display.GetDisplays();

            if (Log.Instance.IsTraceEnabled)
            {
                Log.Instance.Trace($"Found displays:");
                foreach (var display in displays)
                    Log.Instance.Trace($" - {display}");
            }

            foreach (var display in Display.GetDisplays())
                if (await IsInternalAsync(display))
                    return display;
            return null;
        }

        private static async Task<bool> IsInternalAsync(Display display)
        {
            var instanceName = display.DevicePath
                .Split("#")
                .Skip(1)
                .Take(2)
                .Aggregate((s1, s2) => s1 + "\\" + s2);

            var result = await WMI.ReadAsync("root\\WMI",
                             $"SELECT * FROM WmiMonitorConnectionParams WHERE InstanceName LIKE '%{instanceName}%'",
                             pdc => (uint)pdc["VideoOutputTechnology"].Value).ConfigureAwait(false);
            var vot = result.FirstOrDefault();

            const uint votInternal = 0x80000000;
            const uint votDisplayPortEmbedded = 11;
            return vot == votInternal || vot == votDisplayPortEmbedded;
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
}
