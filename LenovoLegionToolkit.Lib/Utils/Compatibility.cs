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
        private static readonly string _allowedModelLine = "Legion";
        private static readonly string[] _allowedModelsPrefix = new[]
        {
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

            "R9000P2021",
            "R9000P2020",
            "R7000P2020",
            "R7000P2021",
            "Y9000K2020",
            "Y9000K2021",
            
            // Limited compatibility
            "17IR",
            "15IR",
        };

        private static MachineInformation? _machineInformation;

        public static async Task<MachineInformation> GetMachineInformation()
        {
            if (!_machineInformation.HasValue)
            {
                var (vendor, machineType, model, serialNumber) = await GetModelDataAsync().ConfigureAwait(false);
                var biosVersion = await GetBIOSVersionAsync().ConfigureAwait(false);
                var modelYear = await GetModelYearAsync();

                _machineInformation = new()
                {
                    Vendor = vendor,
                    MachineType = machineType,
                    Model = model,
                    SerialNumber = serialNumber,
                    BIOSVersion = biosVersion,
                    ModelYear = modelYear,
                    Properties = new()
                    {
                        ShouldFlipFnLock = GetShouldFlipFnLock(modelYear),
                        SupportsGodMode = GetSupportsGodMode(biosVersion),
                        SupportsACDetection = await GetSupportsACDetection().ConfigureAwait(false),
                        SupportsExtendedHybridMode = GetSupportsExtendedHybridMode(modelYear)
                    }
                };
            }

            return _machineInformation.Value;
        }

        public static async Task<(bool isCompatible, MachineInformation machineInformation)> IsCompatibleAsync()
        {
            var mi = await GetMachineInformation().ConfigureAwait(false);

            if (!mi.Vendor.Equals(_allowedVendor, StringComparison.InvariantCultureIgnoreCase))
                return (false, mi);

            if (!mi.Model.Contains(_allowedModelLine, StringComparison.InvariantCultureIgnoreCase))
                return (false, mi);

            foreach (var allowedModel in _allowedModelsPrefix)
                if (mi.Model.Contains(allowedModel, StringComparison.InvariantCultureIgnoreCase))
                    return (true, mi);

            return (false, mi);
        }

        private static bool GetShouldFlipFnLock(ModelYear modelYear)
        {
            return modelYear == ModelYear.MY2020OrEarlier;
        }

        private static bool GetSupportsGodMode(string biosVersion)
        {
            // 2021
            if (biosVersion.StartsWith("GKCN") && int.TryParse(biosVersion.Replace("GKCN", null).Replace("WW", null), out var rev) && rev >= 49)
                return true;

            // 2021
            if (biosVersion.StartsWith("HHCN") && int.TryParse(biosVersion.Replace("HHCN", null).Replace("WW", null), out rev) && rev >= 23)
                return true;

            // 2022
            // if (biosVersion.StartsWith("K1CN") && int.TryParse(biosVersion.Replace("K1CN", null).Replace("WW", null), out rev) && rev >= 31)
            //    return true;

            // 2022
            // if (biosVersion.StartsWith("JYCN") && int.TryParse(biosVersion.Replace("JYCN", null).Replace("WW", null), out rev) && rev >= 39)
            //     return true;

            return false;
        }

        private static async Task<bool> GetSupportsACDetection()
        {
            return await Power.IsACFitForOC().ConfigureAwait(false) != null;
        }

        private static bool GetSupportsExtendedHybridMode(ModelYear modelYear)
        {
            return modelYear == ModelYear.MY2022OrLater;
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

        private static async Task<ModelYear> GetModelYearAsync()
        {
            if (CheckIf2020OrEarlier())
                return ModelYear.MY2020OrEarlier;

            if (await CheckIf2022OrLaterAsync().ConfigureAwait(false))
                return ModelYear.MY2022OrLater;

            return ModelYear.MY2021;
        }

        private static bool CheckIf2020OrEarlier()
        {
            try
            {
                uint inBuffer = 0x2;
                if (!Native.DeviceIoControl(Devices.GetBattery(), 0x831020E8, ref inBuffer, sizeof(uint), out uint outBuffer, sizeof(uint), out _, IntPtr.Zero))
                {
                    var error = Marshal.GetLastWin32Error();

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"DeviceIoControl returned 0, last error: {error}");

                    throw new InvalidOperationException($"DeviceIoControl returned 0, last error: {error}");
                }

                outBuffer = outBuffer.ReverseEndianness();
                return outBuffer.GetNthBit(19);
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }

        private static async Task<bool> CheckIf2022OrLaterAsync()
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
    }
}
