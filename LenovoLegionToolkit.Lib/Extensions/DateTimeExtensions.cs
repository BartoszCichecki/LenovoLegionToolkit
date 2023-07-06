using System;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class DateTimeExtensions
{
    public static DateTime UtcFrom(int hours, int minutes)
    {
        var now = DateTime.UtcNow;
        return new(now.Year, now.Month, now.Day, hours, minutes, 0, DateTimeKind.Utc);
    }

    public static DateTime LocalFrom(int hours, int minutes)
    {
        var now = DateTime.Now;
        return new(now.Year, now.Month, now.Day, hours, minutes, 0, DateTimeKind.Local);
    }

    public static DateTime UtcDayFrom(DayOfWeek targetDay, int hours, int minutes)
    {
        var now = DateTime.UtcNow;
        var date = new DateTime(now.Year, now.Month, now.Day, hours, minutes, now.Second, DateTimeKind.Utc);
        var daysUntilDayOfWeek = ((int)targetDay - (int)date.DayOfWeek + 7) % 7;
        return date.AddDays(daysUntilDayOfWeek);
    }

    public static DateTime LocalDayFrom(DayOfWeek targetDay, int hours, int minutes)
    {
        var now = DateTime.Now;
        var date = new DateTime(now.Year, now.Month, now.Day, hours, minutes, now.Second, DateTimeKind.Local);
        var daysUntilDayOfWeek = ((int)targetDay - (int)date.DayOfWeek + 7) % 7;
        return date.AddDays(daysUntilDayOfWeek);
    }
}
