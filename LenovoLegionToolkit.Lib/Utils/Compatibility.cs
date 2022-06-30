using System;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Utils
{
    public static class Compatibility
    {
        private static readonly string _allowedVendor = "LENOVO";
        private static readonly string[] _allowedModels = new[]
        {
            "16ACHg6", // Legion 7 - AMD, nVidia
            "16ITHg6", // Legion 7 - Intel, nVidia
            "15IMHg05", // Legion 7 - Intel, nVidia
            "16ACH6", // Legion 5 Pro - AMD, nVidia
            "R7000P2020H", // Legion 5 Pro - AMD, nVidia (CN)
            "Y9000K2020H", // Legioin 7i - Intel, nVidia (CN)
            "16ITH6", // Legion 5 Pro - Intel, nVidia
            "16IAH7", // Legion 5 Pro - Intel, nVidia
            "17ACH6", // Legion 5 - AMD, nVidia
            "17ITH6", // Legion 5 - Intel, nVidia
            "15ACH6", // Legion 5 - AMD, nVidia
            "15ARH05", // Legion 5 - AMD, nVidia
            "15IMH05", // Legion 5 - Intel, nVidia
            "15ITH6", // Legion 5 - Intel, nVidia
            "17IMH05", // Legion 5 - Intel, nVidia
            "R7000P2021H", // Legion 5 - AMD, nVidia (CN)
            "R9000P2021H", // Legion 5 Pro - AMD, nVidia (CN)
            "15ACH6", // Legion S7 - AMD, nVidia

            // Limited compatibility
            "17IRHg", // Legion Y740 - Intel, nVidia
            "15IR", // Legion Y740 - Intel, nVidia
            "15IRH", // Legion Y540 - Intel, nVidia
            "17IRH", // Legion Y540 - Intel, nVidia
        };

        public static async Task<MachineInformation> GetMachineInformation()
        {
            var result = await WMI.ReadAsync("root\\CIMV2",
                            $"SELECT * FROM Win32_ComputerSystemProduct",
                            Create).ConfigureAwait(false);
            return result.First();
        }

        public static async Task<(bool isCompatible, MachineInformation machineInformation)> IsCompatibleAsync()
        {
            var machineInformation = await GetMachineInformation().ConfigureAwait(false);

            if (!machineInformation.Vendor.Equals(_allowedVendor, StringComparison.InvariantCultureIgnoreCase))
                return (false, machineInformation);

            foreach (var allowedModel in _allowedModels)
                if (machineInformation.Model.Contains(allowedModel, StringComparison.InvariantCultureIgnoreCase))
                    return (true, machineInformation);

            return (false, machineInformation);
        }

        private static MachineInformation Create(PropertyDataCollection properties)
        {
            var machineType = (string)properties["Name"].Value;
            var vendor = (string)properties["Vendor"].Value;
            var model = (string)properties["Version"].Value;
            var serialNumber = (string)properties["IdentifyingNumber"].Value;
            return new(vendor, machineType, model, serialNumber);
        }
    }
}
