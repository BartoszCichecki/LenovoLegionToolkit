using System.Linq;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Devices.Display;
using WindowsDisplayAPI;
using WindowsDisplayAPI.DisplayConfig;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class DisplayExtensions
{
    public static void SetSettingsUsingPathInfo(this Display display, DisplaySetting displaySetting)
    {
        // Use display path APIs to change internal display resolution & refresh rate.
        // Compared to Display.SetSettings(), these APIs can change the Active Signal Mode and not just the Desktop mode.
        // Setting 60Hz will change the Active Signal Mode to 60Hz instead of leaving it at the max refresh rate,
        // which lets the display consume less power for more battery life.
        var displaySource = display.ToPathDisplaySource();
        var pathInfos = PathInfo.GetActivePaths();

        for (var i = 0; i < pathInfos.Length; i++)
        {
            var pathInfo = pathInfos[i];

            if (pathInfo.DisplaySource == displaySource)
            {
                var targetsInfo = pathInfo.TargetsInfo;
                var pathTargetInfos = targetsInfo
                    .Select(targetInfo => new PathTargetInfo(targetInfo.DisplayTarget,
                        new PathTargetSignalInfo(displaySetting, displaySetting.Resolution),
                        targetInfo.Rotation,
                        targetInfo.Scaling))
                    .ToArray();

                pathInfos[i] = new PathInfo(
                    pathInfo.DisplaySource,
                    pathInfo.Position,
                    displaySetting.Resolution,
                    pathInfo.PixelFormat,
                    pathTargetInfos
                );
            }
        }

        PathInfo.ApplyPathInfos(pathInfos);
    }

    public static DisplayAdvancedColorInfo GetAdvancedColorInfo(this Display display)
    {
        var pathDisplayAdapter = display.Adapter.ToPathDisplayAdapter();
        var pathDisplayTarget = display.ToPathDisplayTarget();

        if (pathDisplayTarget is null || pathDisplayAdapter is null)
            return default;

        var getAdvancedColorInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
        getAdvancedColorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
        getAdvancedColorInfo.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO));
        getAdvancedColorInfo.header.adapterId.HighPart = pathDisplayAdapter.AdapterId.HighPart;
        getAdvancedColorInfo.header.adapterId.LowPart = pathDisplayAdapter.AdapterId.LowPart;
        getAdvancedColorInfo.header.id = pathDisplayTarget.TargetId;

        if (PInvoke.DisplayConfigGetDeviceInfo(ref getAdvancedColorInfo.header) != 0)
            PInvokeExtensions.ThrowIfWin32Error("GetAdvancedColorInfo");

        return new(getAdvancedColorInfo.Anonymous.value.GetNthBit(0),
            getAdvancedColorInfo.Anonymous.value.GetNthBit(1),
            getAdvancedColorInfo.Anonymous.value.GetNthBit(2),
            getAdvancedColorInfo.Anonymous.value.GetNthBit(3));
    }

    public static void SetAdvancedColorState(this Display display, bool state)
    {
        var pathDisplayAdapter = display.Adapter.ToPathDisplayAdapter();
        var pathDisplayTarget = display.ToPathDisplayTarget();

        if (pathDisplayTarget is null || pathDisplayAdapter is null)
            return;

        var setAdvancedColorState = new DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE();
        setAdvancedColorState.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE;
        setAdvancedColorState.header.size = (uint)Marshal.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE>();
        setAdvancedColorState.header.adapterId.HighPart = pathDisplayAdapter.AdapterId.HighPart;
        setAdvancedColorState.header.adapterId.LowPart = pathDisplayAdapter.AdapterId.LowPart;
        setAdvancedColorState.header.id = pathDisplayTarget.TargetId;
        setAdvancedColorState.Anonymous.value = setAdvancedColorState.Anonymous.value.SetNthBit(0, state);

        if (PInvoke.DisplayConfigSetDeviceInfo(setAdvancedColorState.header) != 0)
            PInvokeExtensions.ThrowIfWin32Error("SetAdvancedColorState");
    }
}
