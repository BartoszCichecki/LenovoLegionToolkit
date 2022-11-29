using System;

namespace LenovoLegionToolkit.Lib.Extensions;

public class DateTimeExtensions
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
}