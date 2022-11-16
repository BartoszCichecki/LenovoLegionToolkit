using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

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

        public static RGBColor GetThemeMatchedAccentColor() =>
            GetUxThemeImmersiveColor(GetDarkMode() ? "SystemAccentLight2" : "SystemAccentDark1");

        public static RGBColor GetUxThemeImmersiveColor(string name)
        {
            uint colorType = GetImmersiveColorTypeFromName("Immersive" + name);

            if (colorType == 0xFFFFFFFF)
                throw new Win32Exception($"Couldn't get color \"{name}\"");

            uint activeColorSet = GetImmersiveUserColorSetPreference(false, false);

            uint nativeColor = GetImmersiveColorFromColorSetEx(activeColorSet, colorType, false, 0);

            return new((byte)((0x000000FF & nativeColor) >> 0),
                (byte)((0x0000FF00 & nativeColor) >> 8),
                (byte)((0x00FF0000 & nativeColor) >> 16));
        }

        internal static RGBColor GetAccentColorReg()
        {
            var registryValue = Registry.Read(RegistryHive, DwmRegistryPath, DwmColorizationColorRegistryKey, -1);

            if (registryValue == -1)
                throw new InvalidOperationException($"Couldn't read the {DwmColorizationColorRegistryKey} setting.");

            var registryValueBytes = BitConverter.GetBytes(registryValue);

            return new(registryValueBytes[2], registryValueBytes[1], registryValueBytes[0]);
        }

        internal static IDisposable GetDarkModeListener(Action callback) =>
            Registry.Listen(RegistryHive, PersonalizeRegistryPath, AppsUseLightThemeRegistryKey, callback);

        internal static IDisposable GetAccentColorListener(Action callback) =>
             Registry.Listen(RegistryHive, DwmRegistryPath, DwmColorizationColorRegistryKey, callback);

        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        private static extern uint GetImmersiveColorFromColorSetEx(uint immersiveColorSet, uint immersiveColorType,
            bool ignoreHighContrast, uint highContrastCacheMode);

        [DllImport("uxtheme.dll", EntryPoint = "#96", CharSet = CharSet.Auto)]
        private static extern uint GetImmersiveColorTypeFromName(string name);

        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        private static extern uint GetImmersiveUserColorSetPreference(bool forceCheckRegistry, bool skipCheckOnFail);
    }
}
