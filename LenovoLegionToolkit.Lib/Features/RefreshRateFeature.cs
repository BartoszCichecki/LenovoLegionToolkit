using System;
using System.Linq;
using LenovoLegionToolkit.Lib.Utils;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Features
{
    public class RefreshRateFeature : IDynamicFeature<RefreshRate>
    {
        public RefreshRate[] GetAllStates()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting all refresh rates...");

            var display = GetBuiltInDisplay();
            if (display == null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Built in display not found");

                throw new InvalidOperationException("Built in display not found");
            }

            var currentSettings = display.CurrentSetting;
            var result = display.GetPossibleSettings()
                .Where(dps => Match(dps, currentSettings))
                .Select(dps => new RefreshRate(dps.Frequency))
                .ToArray();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Possible refresh rates are {string.Join(", ", result)}");

            return result;
        }

        public RefreshRate GetState()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting current refresh rate...");

            var display = GetBuiltInDisplay();
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

        public void SetState(RefreshRate state)
        {
            var display = GetBuiltInDisplay();
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

        private static Display GetBuiltInDisplay()
        {
            return Display.GetDisplays()
                .Where(IsInternal)
                .FirstOrDefault();
        }

        private static bool IsInternal(Display display)
        {
            var instanceName = display.DevicePath
                .Split("#")
                .Skip(1)
                .Take(2)
                .Aggregate((s1, s2) => s1 + "\\" + s2);

            var vot = WMI.Read("root\\WMI",
                $"SELECT * FROM WmiMonitorConnectionParams WHERE InstanceName LIKE '%{instanceName}%'",
                pdc => (uint)pdc["VideoOutputTechnology"].Value).FirstOrDefault();

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
