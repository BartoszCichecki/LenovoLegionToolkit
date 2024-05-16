using System;
using System.ComponentModel;
using System.Globalization;

namespace LenovoLegionToolkit.Lib.Macro.Utils.TypeConverters;

public class MacroIdentifierTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string str)
            return base.ConvertFrom(context, culture, value);

        var split = str.Split(',');
        if (split.Length != 2)
            return base.ConvertFrom(context, culture, value);

        if (!Enum.TryParse<MacroSource>(split[0], out var source))
            return base.ConvertFrom(context, culture, value);

        if (!ulong.TryParse(split[1], out var key))
            return base.ConvertFrom(context, culture, value);

        return new MacroIdentifier(source, key);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string) || value is not MacroIdentifier macroIdentifier)
            return base.ConvertTo(context, culture, value, destinationType);

        return $"{macroIdentifier.Source},{macroIdentifier.Key}";
    }
}
