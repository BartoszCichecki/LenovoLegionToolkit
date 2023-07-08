using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System;

public static partial class WMI
{
    public static class Win32
    {
        public static IDisposable ListenProcessStartTrace(Action<string, int> handler) => Listen("root\\CIMV2",
            $"SELECT * FROM Win32_ProcessStartTrace",
            pdc =>
            {
                var processName = (string)pdc["ProcessName"].Value;
                var processId = Convert.ToInt32(pdc["ProcessID"].Value);
                handler(processName, processId);
            });
        public static IDisposable ListenProcessStopTrace(Action<string, int> handler) => Listen("root\\CIMV2",
            $"SELECT * FROM Win32_ProcessStopTrace",
            pdc =>
            {
                var processName = (string)pdc["ProcessName"].Value;
                var processId = Convert.ToInt32(pdc["ProcessID"].Value);
                handler(processName, processId);
            });

        public static async Task<(string vendor, string name, string version, string identifyingNumber)> GetComputerSystemProductAsync()
        {
            var result = await ReadAsync("root\\CIMV2",
                $"SELECT * FROM Win32_ComputerSystemProduct",
                pdc =>
                {
                    var vendor = (string)pdc["Vendor"].Value;
                    var name = (string)pdc["Name"].Value;
                    var version = (string)pdc["Version"].Value;
                    var identifyingNumber = (string)pdc["IdentifyingNumber"].Value;
                    return (vendor, name, version, identifyingNumber);
                }).ConfigureAwait(false);
            return result.First();
        }

        public static async Task<string> GetBIOSNameAsync()
        {
            var result = await ReadAsync("root\\CIMV2",
                $"SELECT * FROM Win32_BIOS",
                pdc => (string)pdc["Name"].Value).ConfigureAwait(false);
            return result.First();
        }

        public static async Task<int> GetProcessorAddressWidthAsync()
        {
            var result = await ReadAsync("root\\CIMV2",
                $"SELECT * FROM Win32_Processor",
                pdc => Convert.ToInt32(pdc["AddressWidth"].Value)).ConfigureAwait(false);
            return result.First();
        }

        public static async Task<string> GetOperatingSystemBuildNumberAsync()
        {
            var result = await ReadAsync("root\\CIMV2",
                $"SELECT * FROM Win32_OperatingSystem",
                pdc => (string)pdc["BuildNumber"].Value);
            return result.First();
        }

        public static Task<IEnumerable<DriverInfo>> GetPnpSignedDriversAsync() => ReadAsync("root\\CIMV2",
            $"SELECT * FROM Win32_PnPSignedDriver",
            pdc =>
            {
                var deviceId = pdc["DeviceID"].Value as string ?? string.Empty;
                var hardwareId = pdc["HardWareId"].Value as string ?? string.Empty;
                var driverVersionString = pdc["DriverVersion"].Value as string;
                var driverDateString = pdc["DriverDate"].Value as string;

                Version? driverVersion = null;
                if (Version.TryParse(driverVersionString, out var v))
                    driverVersion = v;

                DateTime? driverDate = null;
                if (driverDateString is not null)
                    driverDate = ManagementDateTimeConverter.ToDateTime(driverDateString).Date;

                return new DriverInfo
                {
                    DeviceId = deviceId,
                    HardwareId = hardwareId,
                    Version = driverVersion,
                    Date = driverDate
                };
            });
    }
}
