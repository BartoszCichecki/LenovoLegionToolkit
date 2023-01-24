using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.System;

public static class InternalDisplay
{
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

            foreach (var display in displays)
            {
                if (await display.IsInternalAsync().ConfigureAwait(false))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Internal display found: {display}");

                    return (_displayHolder = display);
                }
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Internal display not found");

            return (_displayHolder = null);
        }
    }

    private static async Task<bool> IsInternalAsync(this Device display)
    {
        var instanceName = display.DevicePath
            .Split("#")
            .Skip(1)
            .Take(1)
            .Aggregate((s1, s2) => s1 + "\\" + s2);


        var hasBiosName = await WMI.CallAsync("root\\CIMV2", $"SELECT * FROM Win32_PnPEntity WHERE PnPDeviceID LIKE '%{instanceName}%'",
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

        if (!hasBiosName)
            return false;

        var result = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM WmiMonitorConnectionParams WHERE InstanceName LIKE '%{instanceName}%'",
            pdc => (uint)pdc["VideoOutputTechnology"].Value).ConfigureAwait(false);

        const uint votInternal = 0x80000000;
        const uint votDisplayPortEmbedded = 11;

        var resultEnumerated = result.ToArray();

        if (resultEnumerated.Length == 1)
            return resultEnumerated.All(vot => vot is votInternal or votDisplayPortEmbedded);

        if (resultEnumerated.Length == 2)
            return resultEnumerated.Any(vot => vot is votInternal or votDisplayPortEmbedded);

        return false;
    }
}