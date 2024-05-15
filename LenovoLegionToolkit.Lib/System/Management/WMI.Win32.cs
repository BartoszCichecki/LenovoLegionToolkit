using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class Win32
    {
        public static class ProcessStartTrace
        {
            public static IDisposable Listen(Action<int, string> handler) => WMI.Listen("root\\CIMV2",
                $"SELECT * FROM Win32_ProcessStartTrace",
                pdc =>
                {
                    var processId = Convert.ToInt32(pdc["ProcessID"].Value);
                    var processName = (string)pdc["ProcessName"].Value;
                    handler(processId, Path.GetFileNameWithoutExtension(processName));
                });
        }

        public static class ProcessStopTrace
        {
            public static IDisposable Listen(Action<int, string> handler) => WMI.Listen("root\\CIMV2",
                $"SELECT * FROM Win32_ProcessStopTrace",
                pdc =>
                {
                    var processId = Convert.ToInt32(pdc["ProcessID"].Value);
                    var processName = (string)pdc["ProcessName"].Value;
                    handler(processId, Path.GetFileNameWithoutExtension(processName));
                });
        }

        public static class ComputerSystemProduct
        {
            public static async Task<(string vendor, string name, string version, string identifyingNumber)> ReadAsync()
            {
                var result = await WMI.ReadAsync("root\\CIMV2",
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
        }

        public static class Processor
        {
            public static async Task<int> GetAddressWidthAsync()
            {
                var result = await ReadAsync("root\\CIMV2",
                    $"SELECT * FROM Win32_Processor",
                    pdc => Convert.ToInt32(pdc["AddressWidth"].Value)).ConfigureAwait(false);
                return result.First();
            }
        }

        public static class OperatingSystem
        {
            public static async Task<string> GetBuildNumberAsync()
            {
                var result = await ReadAsync("root\\CIMV2",
                    $"SELECT * FROM Win32_OperatingSystem",
                    pdc => (string)pdc["BuildNumber"].Value).ConfigureAwait(false);
                return result.First();
            }
        }

        public static class PnpEntity
        {
            public static async Task<string?> GetDeviceIDAsync(string pnpDeviceIdPart)
            {
                var results = await ReadAsync("root\\CIMV2",
                    $"SELECT * FROM Win32_PnpEntity WHERE DeviceID LIKE '{pnpDeviceIdPart}%'",
                    pdc => (string)pdc["DeviceID"].Value).ConfigureAwait(false);
                return results.FirstOrDefault();
            }
        }

        public static class PnpSignedDriver
        {
            public static Task<IEnumerable<DriverInfo>> ReadAsync() => WMI.ReadAsync("root\\CIMV2",
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

                    return new DriverInfo(deviceId, hardwareId, driverVersion, driverDate);
                });
        }
    }
}
