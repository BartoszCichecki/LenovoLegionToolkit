using System;

namespace LenovoLegionToolkit.Lib.Utils
{
    public static class Compatibility
    {
        private static readonly string _allowedVendor = "LENOVO";
        private static readonly string[] _allowedModels = new[]
        {
            "16ACHg6", //Legion 7 - AMD, nVidia
            "16ITHg6", //Lenovo 7 - Intel, nVidia
            "16ACH6H", //Legion 5 Pro - AMD, nVidia
        };

        public static bool IsCompatible(MachineInformation machineInformation)
        {
            if (machineInformation.Vendor.Equals(_allowedVendor, StringComparison.OrdinalIgnoreCase))
                return false;

            foreach (string allowedModel in _allowedModels)
            {
                if (!machineInformation.Model.Contains(allowedModel, StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }

            return false;
        }
    }
}
