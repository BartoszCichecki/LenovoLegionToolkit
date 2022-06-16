using System;
using System.Globalization;
using System.Reflection;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class AssemblyExtenensions
    {
        public static DateTime? GetBuildDateTime(this Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                    if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                        return result;
                }
            }

            return null;
        }
    }
}
