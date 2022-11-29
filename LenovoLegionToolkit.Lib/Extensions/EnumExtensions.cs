using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var displayAttribute = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttributes(false)
            .OfType<DisplayAttribute>()
            .FirstOrDefault();

        if (displayAttribute?.Name is null)
            return enumValue.ToString();

        if (displayAttribute.ResourceType?.GetProperty(displayAttribute.Name, BindingFlags.Static | BindingFlags.Public)?.GetValue(null) is string str)
            return str;

        return displayAttribute.Name;
    }
}