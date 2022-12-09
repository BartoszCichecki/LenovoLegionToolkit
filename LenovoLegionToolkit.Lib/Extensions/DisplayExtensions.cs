using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Devices.Display;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class DisplayExtensions
{
    public static async Task<Display?> GetBuiltInDisplayAsync()
    {
        var displays = Display.GetDisplays();

        if (Log.Instance.IsTraceEnabled)
        {
            Log.Instance.Trace($"Found displays:");
            foreach (var display in displays)
                Log.Instance.Trace($" - {display}");
        }

        foreach (var display in Display.GetDisplays())
            if (await display.IsInternalAsync().ConfigureAwait(false))
                return display;

        return null;
    }

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

    public static unsafe DisplaScaleInfo GetDisplaScaleInfo(this Display display)
    {
        var dpiInfo = new DisplaScaleInfo();
        dpiInfo.mininum = 100;
        dpiInfo.maximum = (uint)display.ToPathDisplaySource().MaximumDPIScale;
        dpiInfo.current = (uint)display.ToPathDisplaySource().CurrentDPIScale;
        dpiInfo.recommended = (uint)display.ToPathDisplaySource().RecommendedDPIScale;
        return dpiInfo;
    }

    private static async Task<bool> IsInternalAsync(this Device display)
    {
        var instanceName = display.DevicePath
            .Split("#")
            .Skip(1)
            .Take(2)
            .Aggregate((s1, s2) => s1 + "\\" + s2);

        var result = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM WmiMonitorConnectionParams WHERE InstanceName LIKE '%{instanceName}%'",
            pdc => (uint)pdc["VideoOutputTechnology"].Value).ConfigureAwait(false);
        var vot = result.FirstOrDefault();

        const uint votInternal = 0x80000000;
        const uint votDisplayPortEmbedded = 11;
        return vot is votInternal or votDisplayPortEmbedded;
    }
}