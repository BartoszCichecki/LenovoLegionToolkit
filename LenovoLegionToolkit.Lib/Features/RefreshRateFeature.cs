using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using LenovoLegionToolkit.Lib.Utils;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Features
{
    public class RefreshRateFeature : IDynamicFeature<RefreshRate>
    {
        private class PNPEntity
        {
            public string DeviceID { get; }
            public string Name { get; }

            public PNPEntity(string deviceID, string name)
            {
                DeviceID = deviceID;
                Name = name;
            }
        }

        public RefreshRate[] GetAllStates()
        {
            var display = GetBuiltInDisplay();
            if (display == null)
                throw new InvalidOperationException("Built in display not found.");

            var currentSettings = display.CurrentSetting;
            return display.GetPossibleSettings()
                .Where(dps => Match(dps, currentSettings))
                .Select(dps => new RefreshRate(dps.Frequency))
                .ToArray();
        }

        public RefreshRate GetState()
        {
            var display = GetBuiltInDisplay();
            if (display == null)
                throw new InvalidOperationException("Built in display not found.");

            return new RefreshRate(display.CurrentSetting.Frequency);
        }

        public void SetState(RefreshRate state)
        {
            var display = GetBuiltInDisplay();
            if (display == null)
                throw new InvalidOperationException("Built in display not found.");

            var currentSettings = display.CurrentSetting;

            if (currentSettings.Frequency == state.Frequency)
                return;

            var newSettings = display.GetPossibleSettings()
                .Where(dps => Match(dps, currentSettings))
                .Select(dps => new DisplaySetting(dps, currentSettings.Position))
                .FirstOrDefault(dps => dps.Frequency == state.Frequency);

            if (newSettings != null)
                display.SetSettings(newSettings, true);
        }

        private static Display GetBuiltInDisplay()
        {
            var displays = Display.GetDisplays();
            var entity = GetBuiltInMonitorEntity();

            if (displays == null || entity == null)
                return null;

            return displays.FirstOrDefault(display => Match(display, entity));
        }

        private static PNPEntity GetBuiltInMonitorEntity()
        {
            return GetAllMonitorEntities()
                .Where(EntityHasBIOSNameProperty)
                .FirstOrDefault();
        }

        private static IEnumerable<PNPEntity> GetAllMonitorEntities()
        {
            return WMI.Read("root\\CIMV2",
                $"SELECT * FROM Win32_PnpEntity WHERE PNPClass='Monitor'",
                Create);
        }

        private static bool EntityHasBIOSNameProperty(PNPEntity monitor)
        {
            var parameters = new object[] { new[] { "DEVPKEY_Device_BiosDeviceName" }, null };
            WMI.Invoke("root\\CIMV2", "Win32_PnpEntity",
                "DeviceID", $"{monitor.DeviceID}",
                "GetDeviceProperties", parameters);

            var results = (ManagementBaseObject[])parameters[1];
            foreach (var result in results)
            {
                var value = result.Properties["Data"]?.Value;
                if (value != null)
                    return true;
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
            var entityMarkers = pnpEntity.DeviceID.Split("\\")
                .Skip(1)
                .Select(s => s.ToUpperInvariant());
            var displayMarkers = display.DevicePath.Split("#")
                .Skip(1)
                .Take(2)
                .Select(s => s.ToUpperInvariant());
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
