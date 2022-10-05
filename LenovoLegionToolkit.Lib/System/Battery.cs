using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.System.Power;

namespace LenovoLegionToolkit.Lib.System
{
    public static class Battery
    {
        public static BatteryInformation GetBatteryInformation()
        {
            var powerStatus = GetSystemPowerStatus();

            var batteryTag = GetBatteryTag();
            var information = GetBatteryInformationEx(batteryTag);
            var status = GetBatteryStatusEx(batteryTag);
            var onBatterySince = GetOnBatterySince();

            double? temperatureC = null;
            DateTime? manufactureDate = null;
            DateTime? firstUseDate = null;
            try
            {
                var lenovoBatteryInformation = FindLenovoBatteryInformation();
                if (lenovoBatteryInformation.HasValue)
                {
                    temperatureC = DecodeTemperatureC(lenovoBatteryInformation.Value.Temperature);
                    manufactureDate = DecodeDateTime(lenovoBatteryInformation.Value.ManufactureDate);
                    firstUseDate = DecodeDateTime(lenovoBatteryInformation.Value.FirstUseDate);
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to get temperature of battery.", ex);
            }

            return new()
            {
                IsCharging = powerStatus.ACLineStatus == 1,
                BatteryPercentage = powerStatus.BatteryLifePercent,
                OnBatterySince = onBatterySince,
                BatteryLifeRemaining = (int)powerStatus.BatteryLifeTime,
                FullBatteryLifeRemaining = (int)powerStatus.BatteryFullLifeTime,
                DischargeRate = status.Rate,
                EstimateChargeRemaining = (int)status.Capacity,
                DesignCapacity = information.DesignedCapacity,
                FullChargeCapacity = information.FullChargedCapacity,
                CycleCount = information.CycleCount,
                BatteryTemperatureC = temperatureC,
                ManufactureDate = manufactureDate,
                FirstUseDate = firstUseDate
            };
        }

        private static SYSTEM_POWER_STATUS GetSystemPowerStatus()
        {
            var result = PInvoke.GetSystemPowerStatus(out var sps);

            if (!result)
                PInvokeExtensions.ThrowIfWin32Error("GetSystemPowerStatus");

            return sps;
        }

        private static uint GetBatteryTag()
        {
            var result = PInvokeExtensions.DeviceIoControl(Devices.GetBattery(),
                PInvokeExtensions.IOCTL_BATTERY_QUERY_TAG,
                0u,
                out uint tag);

            if (!result)
                PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_TAG");

            return tag;
        }

        private static BatteryInformationEx GetBatteryInformationEx(uint batteryTag)
        {
            var queryInformation = new BatteryQueryInformationEx
            {
                BatteryTag = batteryTag,
                InformationLevel = BatteryQueryInformationLevelEx.BatteryInformation,
            };

            var result = PInvokeExtensions.DeviceIoControl(Devices.GetBattery(),
                PInvokeExtensions.IOCTL_BATTERY_QUERY_INFORMATION,
                queryInformation,
                out BatteryInformationEx bi);

            if (!result)
                PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_INFORMATION");
            return bi;
        }

        private static BatteryStatusEx GetBatteryStatusEx(uint batteryTag)
        {
            var waitStatus = new BatteryWaitStatusEx
            {
                BatteryTag = batteryTag,
            };
            var result = PInvokeExtensions.DeviceIoControl(Devices.GetBattery(),
                PInvokeExtensions.IOCTL_BATTERY_QUERY_STATUS,
                                                waitStatus,
                                                out BatteryStatusEx s);

            if (!result)
                PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_STATUS");

            return s;
        }

        private static LenovoBatteryInformationEx? FindLenovoBatteryInformation()
        {
            for (uint index = 0; index < 3; index++)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Checking battery data at index {index}...");

                var info = GetLenovoBatteryInformation(index);
                if (info.Temperature is ushort.MinValue or ushort.MaxValue)
                    continue;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Battery data found at index {index}.");

                return info;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Battery data not found.");

            return null;
        }

        private static LenovoBatteryInformationEx GetLenovoBatteryInformation(uint index)
        {
            var result = PInvokeExtensions.DeviceIoControl(Drivers.GetEnergy(),
                                                Drivers.IOCTL_ENERGY,
                                                index,
                                                out LenovoBatteryInformationEx bi);
            if (!result)
                PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, 0x83102138");

            return bi;
        }
        private static DateTime? GetOnBatterySince()
        {
            try
            {
                var logs = new List<(DateTime Date, bool IsACOnline)>();

                var query = new EventLogQuery("System", PathType.LogName, "*[System[EventID=105]]");
                using var logReader = new EventLogReader(query);
                using var propertySelector = new EventLogPropertySelector(new[] { "Event/EventData/Data[@Name='AcOnline']" });

                while (logReader.ReadEvent() is EventLogRecord record)
                {
                    var date = record.TimeCreated;
                    var isAcOnline = record.GetPropertyValues(propertySelector)[0] as bool?;

                    if (date is null || isAcOnline is null)
                        continue;

                    logs.Add((date.Value, isAcOnline.Value));
                }

                if (logs.Count < 1)
                    return null;

                var (dateTime, isACOnline) = logs.MaxBy(l => l.Date);
                if (!isACOnline)
                    return dateTime;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to get event.", ex);
            }

            return null;
        }

        private static DateTime? DecodeDateTime(ushort s)
        {
            try
            {
                var date = new DateTime((s >> 9) + 1980, (s >> 5) & 15, (s & 31), 0, 0, 0, DateTimeKind.Unspecified);
                if (date.Year is < 2018 or > 2026)
                    return null;
                return date;
            }
            catch
            {
                return null;
            }
        }

        private static double? DecodeTemperatureC(ushort s)
        {
            var value = (s - 2731.6) / 10.0;
            if (value < 0)
                return null;
            return value;
        }
    }
}
