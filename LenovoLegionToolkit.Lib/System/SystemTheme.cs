﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.System;

public static partial class SystemTheme
{
    private const string REGISTRY_HIVE = "HKEY_CURRENT_USER";

    private const string PERSONALIZE_REGISTRY_PATH = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string APPS_USE_LIGHT_THEME_REGISTRY_KEY = "AppsUseLightTheme";

    private const string DWM_REGISTRY_PATH = @"Software\Microsoft\Windows\DWM";
    private const string DWM_COLORIZATION_COLOR_REGISTRY_KEY = "ColorizationColor";

    public static bool IsDarkMode()
    {
        var registryValue = Registry.GetValue(REGISTRY_HIVE, PERSONALIZE_REGISTRY_PATH, APPS_USE_LIGHT_THEME_REGISTRY_KEY, -1);
        if (registryValue == -1)
            throw new InvalidOperationException($"Couldn't read the {APPS_USE_LIGHT_THEME_REGISTRY_KEY} setting");

        return registryValue == 0;
    }

    public static RGBColor GetColorizationColor()
    {
        var registryValue = Registry.GetValue(REGISTRY_HIVE, DWM_REGISTRY_PATH, DWM_COLORIZATION_COLOR_REGISTRY_KEY, -1);
        if (registryValue == -1)
            throw new InvalidOperationException($"Couldn't read the {DWM_COLORIZATION_COLOR_REGISTRY_KEY} setting");

        var bytes = BitConverter.GetBytes(registryValue);
        return new(bytes[2], bytes[1], bytes[0]);
    }

    public static RGBColor GetAccentColor()
    {
        var colorName = IsDarkMode() ? "SystemAccentLight2" : "SystemAccentDark1";
        return GetUxThemeImmersiveColor(colorName);
    }

    private static RGBColor GetUxThemeImmersiveColor(string name)
    {
        var colorType = GetImmersiveColorTypeFromName("Immersive" + name);

        if (colorType == 0xFFFFFFFF)
            throw new Win32Exception($"Couldn't get color \"{name}\"");

        var activeColorSet = GetImmersiveUserColorSetPreference(false, false);
        var nativeColor = GetImmersiveColorFromColorSetEx(activeColorSet, colorType, false, 0);

        var r = (byte)((0x000000FF & nativeColor) >> 0);
        var g = (byte)((0x0000FF00 & nativeColor) >> 8);
        var b = (byte)((0x00FF0000 & nativeColor) >> 16);

        return new(r, g, b);
    }

    internal static IDisposable GetDarkModeListener(Action callback)
    {
        return Registry.ObserveValue(REGISTRY_HIVE, PERSONALIZE_REGISTRY_PATH, APPS_USE_LIGHT_THEME_REGISTRY_KEY, callback);
    }

    internal static IDisposable GetColorizationColorListener(Action callback)
    {
        return Registry.ObserveValue(REGISTRY_HIVE, DWM_REGISTRY_PATH, DWM_COLORIZATION_COLOR_REGISTRY_KEY, callback);
    }

    #region Native

    // ReSharper disable StringLiteralTypo

    [LibraryImport("uxtheme.dll", EntryPoint = "#95A")]
    private static partial uint GetImmersiveColorFromColorSetEx(uint immersiveColorSet, uint immersiveColorType, [MarshalAs(UnmanagedType.Bool)] bool ignoreHighContrast, uint highContrastCacheMode);

    [LibraryImport("uxtheme.dll", EntryPoint = "#96W", StringMarshalling = StringMarshalling.Utf16)]
    private static partial uint GetImmersiveColorTypeFromName(string name);

    [LibraryImport("uxtheme.dll", EntryPoint = "#98A")]
    private static partial uint GetImmersiveUserColorSetPreference([MarshalAs(UnmanagedType.Bool)] bool forceCheckRegistry, [MarshalAs(UnmanagedType.Bool)] bool skipCheckOnFail);

    // ReSharper restore StringLiteralTypo

    #endregion
}
