using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LenovoLegionToolkit.Lib.Extensions
{
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
            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}
