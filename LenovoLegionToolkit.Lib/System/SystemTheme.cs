using System;

namespace LenovoLegionToolkit.Lib.System
{
    public static class SystemTheme
    {
        private const string RegistryHive = "HKEY_CURRENT_USER";

        private const string PersonalizeRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string AppsUseLightThemeRegistryKey = "AppsUseLightTheme";

        private const string DwmRegistryPath = @"Software\Microsoft\Windows\DWM";
        private const string DwmColorizationColorRegistryKey = "ColorizationColor";

        public static bool GetDarkMode()
        {
            var registryValue = Registry.Read(RegistryHive, PersonalizeRegistryPath, AppsUseLightThemeRegistryKey, -1);
            if (registryValue == -1)
                throw new InvalidOperationException($"Couldn't read the {AppsUseLightThemeRegistryKey} setting.");

            return registryValue == 0;
        }

        public static RGBColor GetAccentColor()
        {
            var registryValue = Registry.Read(RegistryHive, DwmRegistryPath, DwmColorizationColorRegistryKey, -1);

            if (registryValue == -1)
                throw new InvalidOperationException($"Couldn't read the {DwmColorizationColorRegistryKey} setting.");

            var registryValueBytes = BitConverter.GetBytes(registryValue);

            return new(registryValueBytes[2], registryValueBytes[1], registryValueBytes[0]);
        }

        internal static IDisposable GetDarkModeListener(Action callback)
        {
            return Registry.Listen(RegistryHive, PersonalizeRegistryPath, AppsUseLightThemeRegistryKey, callback);
        }

        internal static IDisposable GetAccentColorListener(Action callback)
        {
            return Registry.Listen(RegistryHive, DwmRegistryPath, DwmColorizationColorRegistryKey, callback);
        }
    }
}
