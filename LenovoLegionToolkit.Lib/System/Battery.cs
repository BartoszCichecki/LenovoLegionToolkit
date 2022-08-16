using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System
{
    public static class Battery
    {
        public static async Task<BatteryInformation> GetBatteryInformationAsync()
        {
            var powerStatus = GetSystemPowerStatus();

            var batteryTag = GetBatteryTag();
            var information = GetBatteryInformationEx(batteryTag);
            var status = GetBatteryStatusEx(batteryTag);

            double? temperatureC = null;
            try
            {
                var lenovoInformation = GetLenovoBatteryInformation();
                temperatureC = (lenovoInformation.Temperature - 2731.6) / 10.0;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to get temperature of battery.", ex);
            }

            DateTime? manufactureDate = null;
            try
            {
                manufactureDate = await GetBatteryManufactureDateAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to get manufacture date of battery.", ex);
            }

            return new(powerStatus.ACLineStatus == ACLineStatusEx.Online,
                       powerStatus.BatteryLifePercent,
                       powerStatus.BatteryLifeTime,
                       powerStatus.BatteryFullLifeTime,
                       status.Rate,
                       (int)status.Capacity,
                       information.DesignedCapacity,
                       information.FullChargedCapacity,
                       information.CycleCount,
                       temperatureC,
                       manufactureDate);
        }

        private static SystemPowerStatusEx GetSystemPowerStatus()
        {
            var result = Native.GetSystemPowerStatus(out SystemPowerStatusEx sps);

            if (!result)
                NativeUtils.ThrowIfWin32Error("GetSystemPowerStatus");

            return sps;
        }

        private static uint GetBatteryTag()
        {
            uint emptyInput = 0;
            var result = Native.DeviceIoControl(Devices.GetBattery(),
                Native.IOCTL_BATTERY_QUERY_TAG,
                ref emptyInput,
                4,
                out uint tag,
                4,
                out _,
                IntPtr.Zero);

            if (!result)
                NativeUtils.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_TAG");

            return tag;
        }

        private static BatteryInformationEx GetBatteryInformationEx(uint batteryTag)
        {
            var queryInformationPointer = IntPtr.Zero;
            var informationPointer = IntPtr.Zero;

            try
            {
                var queryInformation = new BatteryQueryInformationEx
                {
                    BatteryTag = batteryTag,
                    InformationLevel = BatteryQueryInformationLevelEx.BatteryInformation,
                };
                var queryInformationSize = Marshal.SizeOf<BatteryQueryInformationEx>();
                queryInformationPointer = Marshal.AllocHGlobal(queryInformationSize);
                Marshal.StructureToPtr(queryInformation, queryInformationPointer, false);

                var informationSize = Marshal.SizeOf<BatteryInformationEx>();
                informationPointer = Marshal.AllocHGlobal(informationSize);

                var result = Native.DeviceIoControl(Devices.GetBattery(),
                                                    Native.IOCTL_BATTERY_QUERY_INFORMATION,
                                                    queryInformationPointer,
                                                    queryInformationSize,
                                                    informationPointer,
                                                    informationSize,
                                                    out _,
                                                    IntPtr.Zero);

                if (!result)
                    NativeUtils.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_INFORMATION");

                var bi = Marshal.PtrToStructure<BatteryInformationEx>(informationPointer);
                return bi;
            }
            finally
            {
                Marshal.FreeHGlobal(queryInformationPointer);
                Marshal.FreeHGlobal(informationPointer);
            }
        }

        private static BatteryStatusEx GetBatteryStatusEx(uint batteryTag)
        {
            var waitStatusPointer = IntPtr.Zero;
            var statusPointer = IntPtr.Zero;

            try
            {
                var waitStatus = new BatteryWaitStatusEx
                {
                    BatteryTag = batteryTag,
                };
                var waitStatusSize = Marshal.SizeOf<BatteryWaitStatusEx>();
                waitStatusPointer = Marshal.AllocHGlobal(waitStatusSize);
                Marshal.StructureToPtr(waitStatus, waitStatusPointer, false);

                var statusSize = Marshal.SizeOf<BatteryStatusEx>();
                statusPointer = Marshal.AllocHGlobal(statusSize);

                var result = Native.DeviceIoControl(Devices.GetBattery(),
                                                    Native.IOCTL_BATTERY_QUERY_STATUS,
                                                    waitStatusPointer,
                                                    waitStatusSize,
                                                    statusPointer,
                                                    statusSize,
                                                    out _,
                                                    IntPtr.Zero);

                if (!result)
                    NativeUtils.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_STATUS");

                var s = Marshal.PtrToStructure<BatteryStatusEx>(statusPointer);
                return s;
            }
            finally
            {
                Marshal.FreeHGlobal(waitStatusPointer);
                Marshal.FreeHGlobal(statusPointer);
            }
        }

        private static LenovoBatteryInformationEx GetLenovoBatteryInformation()
        {
            var batteryInformationPointer = IntPtr.Zero;
            try
            {
                uint emptyBuffer = 0;
                var batteryInformationSize = Marshal.SizeOf<LenovoBatteryInformationEx>();
                batteryInformationPointer = Marshal.AllocHGlobal(batteryInformationSize);
                var result = Native.DeviceIoControl(Drivers.GetEnergy(),
                                                    0x83102138,
                                                    ref emptyBuffer,
                                                    4,
                                                    batteryInformationPointer,
                                                    batteryInformationSize,
                                                    out _,
                                                    IntPtr.Zero);
                if (!result)
                    NativeUtils.ThrowIfWin32Error("DeviceIoControl, 0x83102138");

                var bi = Marshal.PtrToStructure<LenovoBatteryInformationEx>(batteryInformationPointer);
                return bi;
            }
            finally
            {
                Marshal.FreeHGlobal(batteryInformationPointer);
            }
        }

        private static async Task<DateTime?> GetBatteryManufactureDateAsync()
        {
            var result = await WMI.ReadAsync("ROOT\\WMI", $"SELECT * FROM Lenovo_BatteryInformation", pdc => pdc["CurrentSetting"].Value.ToString()).ConfigureAwait(false);

            var enumerator = result.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current is not null && current.StartsWith("BAT0 MfgDate ,"))
                {
                    var dateString = current.GetAfterOrEmpty(",");
                    if (DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                        return dateTime;
                }
            }

            return null;
        }
    }
}
