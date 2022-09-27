﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using LenovoLegionToolkit.Lib.Utils;

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
                IsCharging = powerStatus.ACLineStatus == ACLineStatusEx.Online,
                BatteryPercentage = powerStatus.BatteryLifePercent,
                OnBatterySince = onBatterySince,
                BatteryLifeRemaining = powerStatus.BatteryLifeTime,
                FullBatteryLifeRemaining = powerStatus.BatteryFullLifeTime,
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
            var batteryInformationPointer = IntPtr.Zero;
            try
            {
                var batteryInformationSize = Marshal.SizeOf<LenovoBatteryInformationEx>();
                batteryInformationPointer = Marshal.AllocHGlobal(batteryInformationSize);
                var result = Native.DeviceIoControl(Drivers.GetEnergy(),
                                                    0x83102138,
                                                    ref index,
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
