using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Devices.Display;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class DisplayExtensions
{
    public static DisplayAdvancedColorInfo GetAdvancedColorInfo(this Display display)
    {
        var getAdvancedColorInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
        getAdvancedColorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
        getAdvancedColorInfo.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO));
        getAdvancedColorInfo.header.adapterId.HighPart = display.Adapter.ToPathDisplayAdapter().AdapterId.HighPart;
        getAdvancedColorInfo.header.adapterId.LowPart = display.Adapter.ToPathDisplayAdapter().AdapterId.LowPart;
        getAdvancedColorInfo.header.id = display.ToPathDisplayTarget().TargetId;

        if (PInvoke.DisplayConfigGetDeviceInfo(ref getAdvancedColorInfo.header) != 0)
            PInvokeExtensions.ThrowIfWin32Error("GetAdvancedColorInfo");

        return new(getAdvancedColorInfo.Anonymous.value.GetNthBit(0),
            getAdvancedColorInfo.Anonymous.value.GetNthBit(1),
            getAdvancedColorInfo.Anonymous.value.GetNthBit(2),
            getAdvancedColorInfo.Anonymous.value.GetNthBit(3));
    }

    public static void SetAdvancedColorState(this Display display, bool state)
    {
        var setAdvancedColorState = new DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE();
        setAdvancedColorState.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE;
        setAdvancedColorState.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE));
        setAdvancedColorState.header.adapterId.HighPart = display.Adapter.ToPathDisplayAdapter().AdapterId.HighPart;
        setAdvancedColorState.header.adapterId.LowPart = display.Adapter.ToPathDisplayAdapter().AdapterId.LowPart;
        setAdvancedColorState.header.id = display.ToPathDisplayTarget().TargetId;

        setAdvancedColorState.Anonymous.value = setAdvancedColorState.Anonymous.value.SetNthBit(0, state);

        if (PInvoke.DisplayConfigSetDeviceInfo(setAdvancedColorState.header) != 0)
            PInvokeExtensions.ThrowIfWin32Error("SetAdvancedColorState");
    }
}