using System;
using System.Linq;
using System.Management;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class MachineInformation
    {
        public string Vendor { get; }
        public string Model { get; }
        
        public MachineInformation(string vendor, string model)
        {
            Vendor = vendor;
            Model = model;
        }
    }

    public static class Compatibility
    {
        private static readonly string _allowedVendor = "LENOVO";
        private static readonly string[] _allowedModels = new[]
        {
            "16ACHg6", // Legion 7 - AMD, nVidia
            "16ITHg6", // Legion 7 - Intel, nVidia
            "15IMHg05", // Legion 7 - Intel, nVidia
            "16ACH6", // Legion 5 Pro - AMD, nVidia
            "16ITH6", // Legion 5 Pro - Intel, nVidia
            "17ACH6", // Legion 5 - AMD, nVidia
            "15ACH6", // Legion 5 - AMD, nVidia
            "15ARH05", // Legion 5 - AMD, nVidia
            "15IMH05", // Legion 5 - Intel, nVidia
            "R7000P2021H", // Legion 5 - AMD, nVidia (CN)
            "15ACH6", // Legion S7 - AMD, nVidia
            "17IRHg", // Legion Y740 - Intel, nVidia
            "15IRH", // Legion Y540 - Intel, nVidia
            "17IRH", // Legion Y540 - Intel, nVidia
        };

        public static bool IsCompatible(out MachineInformation machineInformation)
        {
            machineInformation = WMI.Read("root\\CIMV2", "SELECT * FROM Win32_ComputerSystemProduct", Create).First();

            if (!machineInformation.Vendor.Equals(_allowedVendor, StringComparison.OrdinalIgnoreCase))
                return false;

            foreach (var allowedModel in _allowedModels)
                if (machineInformation.Model.Contains(allowedModel, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }

        private static MachineInformation Create(PropertyDataCollection properties)
        {
            var vendor = (string)properties["Vendor"].Value;
            var model = (string)properties["Version"].Value;
            return new(vendor, model);
        }
    }
}
