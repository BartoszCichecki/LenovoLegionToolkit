using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class PNPEntity
    {
        public string DeviceID { get; }
        public string Name { get; }

        public PNPEntity(string deviceID, string name)
        {
            DeviceID = deviceID;
            Name = name;
        }
    }

    public class DisplayModes
    {
        public DisplaySetting Current { get; }
        public DisplaySetting[] Possible { get; }

        public DisplayModes(DisplaySetting current, DisplaySetting[] possible)
        {
            Current = current;
            Possible = possible;
        }
    }

    public static class Monitors
    {
        public static Display GetBuiltInDisplay()
        {
            var displays = Display.GetDisplays();
            var entity = GetBuiltInMonitorEntity();

            return displays.FirstOrDefault(display => Match(display, entity));
        }

        public static DisplayModes GetMainDisplayModes(Display display)
        {
            if (display == null)
                return null;            

            var currentSettings = display.CurrentSetting;
            var possibleSettings = display.GetPossibleSettings()
                .Where(dps => Match(dps, currentSettings))
                .Select(dps => new DisplaySetting(dps, currentSettings.Position))
                .ToArray();

            return new(currentSettings, possibleSettings);
        }

        public static void Activate(Display display, DisplaySetting displaySetting)
        {
            display.SetSettings(displaySetting, true);
        }

        private static PNPEntity GetBuiltInMonitorEntity() => GetAllMonitorEntities().Where(HasBIOSName).FirstOrDefault();

        private static IEnumerable<PNPEntity> GetAllMonitorEntities()
        {
            return WMI.Read("root\\CIMV2", "SELECT * FROM Win32_PnpEntity WHERE PNPClass=\"Monitor\"", Create);
        }

        private static bool HasBIOSName(PNPEntity monitor)
        {
            var parameters = new object[] { new string[] { "DEVPKEY_Device_BiosDeviceName" }, null };
            WMI.InvokeForResult("root\\CIMV2",
                "Win32_PnpEntity",
                "DeviceID",
                monitor.DeviceID,
                "GetDeviceProperties",
                parameters);

            var results = parameters[1] as ManagementBaseObject[];
            foreach (var result in results)
            {
                var value = result.Properties.OfType<PropertyData>().FirstOrDefault(pd => pd.Name == "Data")?.Value;
                return value != null;
            }

            return false;
        }

        private static PNPEntity Create(PropertyDataCollection properties)
        {
            var deviceID = (string)properties["DeviceID"].Value;
            var name = (string)properties["Name"].Value;
            return new(deviceID, name);
        }
        private static bool Match(Display display, PNPEntity pnpEntity)
        {
            var entityMarkers = pnpEntity.DeviceID.Split("\\").Skip(1).Select(s => s.ToUpperInvariant());
            var displayMarkers = display.DevicePath.Split("#").Skip(1).Take(2).Select(s => s.ToUpperInvariant());
            return entityMarkers.SequenceEqual(displayMarkers);
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
