using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.System;

public static class InternalDisplay
{
    private enum VideoOutputTechnology : uint
    {
        Internal = 0x80000000u,
        DisplayPortExternal = 10u,
        DisplayPortEmbedded = 11u,
    }

    private readonly struct DisplayHolder
    {
        public readonly Display? Display;

        public DisplayHolder(Display? display)
        {
            Display = display;
        }

        public static implicit operator DisplayHolder(Display? s) => new(s);

        public static implicit operator Display?(DisplayHolder s) => s.Display;
    }

    private static readonly AsyncLock _lock = new();
    private static DisplayHolder? _displayHolder;

    public static void SetNeedsRefresh()
    {
        using (_lock.Lock())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Resetting holder...");

            _displayHolder = null;
        }
    }

    public static async Task<Display?> GetAsync()
    {
        using (await _lock.LockAsync())
        {
            if (_displayHolder is not null)
                return _displayHolder;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Finding internal display...");

            var displays = Display.GetDisplays().ToArray();

            if (Log.Instance.IsTraceEnabled)
            {
                Log.Instance.Trace($"Found displays:");
                foreach (var display in displays)
                    Log.Instance.Trace($" - {display}");
            }

            var internalDisplay = await FindInternalDisplayAsync(displays);

            if (Log.Instance.IsTraceEnabled)
            {
                if (internalDisplay is null)
                    Log.Instance.Trace($"Internal display not found");
                else
                    Log.Instance.Trace($"Internal display found: {internalDisplay}");
            }

            return (_displayHolder = internalDisplay);
        }
    }

    private static async Task<Display?> FindInternalDisplayAsync(IEnumerable<Display> displays)
    {
        var displayInfos = new List<(Display Display, VideoOutputTechnology Vot)>();

        foreach (var display in displays)
        {
            var instanceName = GetInstanceNamePattern(display);
            if (!await HasBiosNameAsync(instanceName))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Display {display} does not have BIOS name. [instanceName={instanceName}, display.DevicePath={display.DevicePath}]");

                continue;
            }

            var vot = await GetVideoOutputTechnologyAsync(instanceName);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Display {display} is connected over {vot}.");

            displayInfos.Add((display, vot));
        }

        if (displayInfos.Count != 1)
            return null;

        var displayInfo = displayInfos[0];
        return displayInfo.Vot is VideoOutputTechnology.Internal or VideoOutputTechnology.DisplayPortEmbedded or VideoOutputTechnology.DisplayPortExternal
            ? displayInfo.Display
            : null;
    }

    private static string GetInstanceNamePattern(Display display) =>
        display.DevicePath
            .Split("#")
            .Skip(1)
            .Take(1)
            .Aggregate((s1, s2) => s1 + "\\" + s2);

    private static Task<bool> HasBiosNameAsync(string instanceName)
    {
        return WMI.CallAsync("root\\CIMV2", $"SELECT * FROM Win32_PnPEntity WHERE PnPDeviceID LIKE '%{instanceName}%'",
            "GetDeviceProperties",
            new() { { "devicePropertyKeys", new[] { "DEVPKEY_Device_BiosDeviceName" } } },
            pdc =>
            {
                var devicePropertiesObjects = pdc["deviceProperties"].Value as ManagementBaseObject[];
                var properties = devicePropertiesObjects?.FirstOrDefault()?.Properties?.GetEnumerator();
                if (properties is null)
                    return false;

                while (properties.MoveNext())
                {
                    var current = properties.Current;
                    if (current.Name == "Data" && !string.IsNullOrEmpty(current.Value.ToString()))
                        return true;
                }

                return false;
            });
    }

    private static async Task<VideoOutputTechnology> GetVideoOutputTechnologyAsync(string instanceName)
    {
        var result = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM WmiMonitorConnectionParams WHERE InstanceName LIKE '%{instanceName}%'",
            pdc => (uint)pdc["VideoOutputTechnology"].Value).ConfigureAwait(false);
        return (VideoOutputTechnology)result.FirstOrDefault();
    }
}