﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.System.Power;

namespace LenovoLegionToolkit.Lib.System;

public static class Battery
{
    private static readonly ApplicationSettings Settings = IoCContainer.Resolve<ApplicationSettings>();

    public static BatteryInformation GetBatteryInformation()
    {
        var powerStatus = GetSystemPowerStatus();

        var batteryTag = GetBatteryTag();
        var information = GetBatteryInformation(batteryTag);
        var status = GetBatteryStatus(batteryTag);

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

        return new(powerStatus.ACLineStatus == 1,
            powerStatus.BatteryLifePercent,
            (int)powerStatus.BatteryLifeTime,
            (int)powerStatus.BatteryFullLifeTime,
            status.Rate,
            (int)status.Capacity,
            (int)information.DesignedCapacity,
            (int)information.FullChargedCapacity,
            (int)information.CycleCount,
            powerStatus.ACLineStatus == 0 && information.DefaultAlert2 >= status.Capacity,
            temperatureC,
            manufactureDate,
            firstUseDate);
    }

    public static double? GetBatteryTemperatureC()
    {
        try
        {
            var lenovoBatteryInformation = FindLenovoBatteryInformation();
            return lenovoBatteryInformation.HasValue ? DecodeTemperatureC(lenovoBatteryInformation.Value.Temperature) : null;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get temperature of battery.", ex);
            return null;
        }
    }

    public static DateTime? GetOnBatterySince()
    {
        try
        {
            var resetOnReboot = Settings.Store.ResetBatteryOnSinceTimerOnReboot;

            var lastRebootTime = DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount);

            var logs = new List<(DateTime Date, bool IsACOnline)>();

            var query = new EventLogQuery("System", PathType.LogName, "*[System[EventID=105]]");
            using var logReader = new EventLogReader(query);
            using var propertySelector = new EventLogPropertySelector(["Event/EventData/Data[@Name='AcOnline']"]);

            while (logReader.ReadEvent() is EventLogRecord record)
            {
                var date = record.TimeCreated;
                var isAcOnline = record.GetPropertyValues(propertySelector)[0] as bool?;

                if (date is null || isAcOnline is null)
                    continue;

                if (resetOnReboot && date < lastRebootTime)
                    continue;

                logs.Add((date.Value, isAcOnline.Value));
            }

            if (logs.Count < 1)
                return null;

            logs.Reverse();

            var (dateTime, _) = logs
                .TakeWhile(log => log.IsACOnline != true)
                .LastOrDefault();

            return dateTime;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get event.", ex);
        }

        return null;
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
            PInvoke.IOCTL_BATTERY_QUERY_TAG,
            0u,
            out uint tag);

        if (!result)
            PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_TAG");

        return tag;
    }

    private static BATTERY_INFORMATION GetBatteryInformation(uint batteryTag)
    {
        var queryInformation = new BATTERY_QUERY_INFORMATION
        {
            BatteryTag = batteryTag,
            InformationLevel = BATTERY_QUERY_INFORMATION_LEVEL.BatteryInformation,
        };

        var result = PInvokeExtensions.DeviceIoControl(Devices.GetBattery(),
            PInvoke.IOCTL_BATTERY_QUERY_INFORMATION,
            queryInformation,
            out BATTERY_INFORMATION bi);

        if (!result)
            PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_INFORMATION");
        return bi;
    }

    private static BATTERY_STATUS GetBatteryStatus(uint batteryTag)
    {
        var waitStatus = new BATTERY_WAIT_STATUS
        {
            BatteryTag = batteryTag,
        };
        var result = PInvokeExtensions.DeviceIoControl(Devices.GetBattery(),
            PInvoke.IOCTL_BATTERY_QUERY_STATUS,
            waitStatus,
            out BATTERY_STATUS s);

        if (!result)
            PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, IOCTL_BATTERY_QUERY_STATUS");

        return s;
    }

    private static LENOVO_BATTERY_INFORMATION? FindLenovoBatteryInformation()
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

    private static LENOVO_BATTERY_INFORMATION GetLenovoBatteryInformation(uint index)
    {
        var result = PInvokeExtensions.DeviceIoControl(Drivers.GetEnergy(),
            Drivers.IOCTL_ENERGY_BATTERY_INFORMATION,
            index,
            out LENOVO_BATTERY_INFORMATION bi);
        if (!result)
            PInvokeExtensions.ThrowIfWin32Error("DeviceIoControl, 0x83102138");

        return bi;
    }

    private static DateTime? DecodeDateTime(ushort s)
    {
        try
        {
            if (s < 1)
                return null;

            var date = new DateTime((s >> 9) + 1980, (s >> 5) & 15, (s & 31), 0, 0, 0, DateTimeKind.Unspecified);
            if (date.Year is < 2018 or > 2030)
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
