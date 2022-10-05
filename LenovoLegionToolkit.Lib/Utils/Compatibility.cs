using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Utils
{
    public static class Compatibility
    {
        private static readonly string _allowedVendor = "LENOVO";

        private static readonly string[] _allowedModelLines = {
            "Legion",
            "Ideapad"
        };

        private static readonly string[] _allowedModelsPrefix = {
            "17ACH",
            "17ARH",
            "17ITH",
            "17IMH",

            "16ACH",
            "16ARH",
            "16IAH",
            "16IAX",
            "16ITH",

            "15ACH",
            "15ARH",
            "15IAH",
            "15IMH",
            "15ITH",

            "R9000P",
            "R7000P",
            "Y9000K",
            "Y9000P",
            "Y9000X",
            
            // Limited compatibility
            "17IR",
            "15IR"
        };

        private static MachineInformation? _machineInformation;

        public static async Task<MachineInformation> GetMachineInformation()
        {
            if (!_machineInformation.HasValue)
            {
                var (vendor, machineType, model, serialNumber) = await GetModelDataAsync().ConfigureAwait(false);
                var biosVersion = await GetBIOSVersionAsync().ConfigureAwait(false);

                var machineInformation = new MachineInformation
                {
                    Vendor = vendor,
                    MachineType = machineType,
                    Model = model,
                    SerialNumber = serialNumber,
                    BIOSVersion = biosVersion,
                    Properties = new()
                    {
                        ShouldFlipFnLock = GetShouldFlipFnLock(),
                        SupportsGodMode = GetSupportsGodMode(biosVersion),
                        SupportsACDetection = await GetSupportsACDetection().ConfigureAwait(false),
                        SupportsExtendedHybridMode = await GetSupportsExtendedHybridModeAsync().ConfigureAwait(false)
                    }
                };

                if (Log.Instance.IsTraceEnabled)
                {
                    Log.Instance.Trace($"Retrieved machine information:");
                    Log.Instance.Trace($" * Vendor: {machineInformation.Vendor}");
                    Log.Instance.Trace($" * Machine Type: {machineInformation.MachineType}");
                    Log.Instance.Trace($" * Model: {machineInformation.Model}");
                    Log.Instance.Trace($" * SupportsACDetection: {machineInformation.Properties.SupportsACDetection}");
                    Log.Instance.Trace($" * ShouldFlipFnLock: {machineInformation.Properties.ShouldFlipFnLock}");
                    Log.Instance.Trace($" * SupportsGodMode: {machineInformation.Properties.SupportsGodMode}");
                    Log.Instance.Trace($" * SupportsExtendedHybridMode: {machineInformation.Properties.SupportsExtendedHybridMode}");
                }

                _machineInformation = machineInformation;
            }

            return _machineInformation.Value;
        }

        public static async Task<(bool isCompatible, MachineInformation machineInformation)> IsCompatibleAsync()
        {
            var mi = await GetMachineInformation().ConfigureAwait(false);

            if (!mi.Vendor.Equals(_allowedVendor, StringComparison.InvariantCultureIgnoreCase))
                return (false, mi);

            foreach (var allowedModel in _allowedModelLines)
                if (mi.Model.Contains(allowedModel, StringComparison.InvariantCultureIgnoreCase))
                    return (true, mi);

            foreach (var allowedModel in _allowedModelsPrefix)
                if (mi.Model.Contains(allowedModel, StringComparison.InvariantCultureIgnoreCase))
                    return (true, mi);

            return (false, mi);
        }

        private static bool GetShouldFlipFnLock()
        {
            try
            {
                if (PInvokeExtensions.DeviceIoControl(Devices.GetBattery(), 0x831020E8, 2u, out uint result))
                    return result.ReverseEndianness().GetNthBit(19);

                var error = Marshal.GetLastWin32Error();

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"DeviceIoControl returned 0, last error: {error}");

                throw new InvalidOperationException($"DeviceIoControl returned 0, last error: {error}");
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static bool GetSupportsGodMode(string biosVersion)
        {
            (string, int)[] supportedBiosVersions =
            {
                ("GKCN", 49),
                ("H1CN", 49),
                ("HHCN", 23),
                ("K1CN", 31),
                ("J2CN", 40),
                ("JYCN", 39)
            };

            foreach (var (biosPrefix, minimumVersion) in supportedBiosVersions)
            {
                if (biosVersion.StartsWith(biosPrefix) && int.TryParse(biosVersion.Replace(biosPrefix, null).Replace("WW", null), out var rev) && rev >= minimumVersion)
                    return true;
            }

            return false;
        }

        private static async Task<bool> GetSupportsACDetection()
        {
            return await Power.IsACFitForOC().ConfigureAwait(false) != null;
        }

        private static async Task<bool> GetSupportsExtendedHybridModeAsync()
        {
            try
            {
                var result = await WMI.CallAsync("root\\WMI",
                    $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                    "IsSupportIGPUMode",
                    new(),
                    pdc => (uint)pdc["Data"].Value).ConfigureAwait(false);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<(string, string, string, string)> GetModelDataAsync()
        {
            var result = await WMI.ReadAsync("root\\CIMV2",
                                $"SELECT * FROM Win32_ComputerSystemProduct",
                                pdc =>
                                {
                                    var machineType = (string)pdc["Name"].Value;
                                    var vendor = (string)pdc["Vendor"].Value;
                                    var model = (string)pdc["Version"].Value;
                                    var serialNumber = (string)pdc["IdentifyingNumber"].Value;
                                    return (vendor, machineType, model, serialNumber);
                                }).ConfigureAwait(false);
            return result.First();
        }

        private static async Task<string> GetBIOSVersionAsync()
        {
            var result = await WMI.ReadAsync("root\\CIMV2",
                                $"SELECT * FROM Win32_BIOS",
                                pdc => (string)pdc["Name"].Value).ConfigureAwait(false);
            return result.First();
        }
    }
}
