using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Devices.Display;
using Windows.Win32.Foundation;
using WindowsDisplayAPI;
using WindowsDisplayAPI.DisplayConfig;

namespace LenovoLegionToolkit.Lib.System;

public static class InternalDisplay
{
    private readonly struct DisplayHolder
    {
        public static DisplayHolder Empty = new DisplayHolder();

        private readonly Display? _display;

        private DisplayHolder(Display? display) => _display = display;

        public static implicit operator DisplayHolder(Display? s) => new(s);

        public static implicit operator Display?(DisplayHolder s) => s._display;
    }

    private static readonly object _lock = new();
    private static DisplayHolder? _displayHolder;

    public static void SetNeedsRefresh()
    {
        lock (_lock)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Resetting holder...");

            _displayHolder = null;
        }
    }

    public static Display? Get()
    {
        lock (_lock)
        {
            if (_displayHolder is not null)
                return _displayHolder;

            var displays = Display.GetDisplays().ToArray();
            var internalDisplay = FindInternalDisplay(displays);
            if (internalDisplay is not null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Found internal display: {internalDisplay}");

                return (_displayHolder = internalDisplay);
            }

            var aoDisplay = FindInternalAdvancedOptimusDisplay(displays);
            if (aoDisplay is not null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Found internal AO display: {aoDisplay}");

                return (_displayHolder = aoDisplay);
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"No internal displays found.");

            return (_displayHolder = DisplayHolder.Empty);
        }
    }

    private static Display? FindInternalDisplay(IEnumerable<Display> displays)
    {
        return displays.Where(d => d.GetVideoOutputTechnology().IsInternalOutput()).FirstOrDefault();
    }

    private static Display? FindInternalAdvancedOptimusDisplay(IEnumerable<Display> displays)
    {
        var exDpDisplays = displays.Where(di => di.GetVideoOutputTechnology().IsExternalDisplayPortOutput()).ToArray();

        if (exDpDisplays.Length < 1)
            return null;

        var exDpDisplay = exDpDisplays[0];
        var exDpPathDisplayTarget = exDpDisplay.ToPathDisplayTarget();
        var exDpPortDisplayEDID = exDpPathDisplayTarget.EDIDManufactureId;

        var sameDeviceIsOnAnotherAdapter = DisplayAdapter.GetDisplayAdapters()
            .Where(da => da.DevicePath != exDpDisplay.Adapter.DevicePath)
            .SelectMany(da => da.GetDisplayDevices())
            .Select(dd => dd.ToPathDisplayTarget())
            .Any(pdt => pdt.EDIDManufactureId == exDpPortDisplayEDID && pdt.GetVideoOutputTechnology().IsInternalOutput());

        return sameDeviceIsOnAnotherAdapter ? exDpDisplay : null;
    }

    private static DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY GetVideoOutputTechnology(this DisplayDevice displayDevice)
    {
        return GetVideoOutputTechnology(displayDevice.ToPathDisplayTarget());
    }

    private static unsafe DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY GetVideoOutputTechnology(this PathDisplayTarget pathDisplayTarget)
    {
        var intPtr = IntPtr.Zero;
        try
        {
            var deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                {
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME,
                    id = pathDisplayTarget.TargetId,
                    adapterId = new LUID
                    {
                        HighPart = pathDisplayTarget.Adapter.AdapterId.HighPart,
                        LowPart = pathDisplayTarget.Adapter.AdapterId.LowPart,
                    },
                    size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>()
                }
            };

            intPtr = Marshal.AllocHGlobal((int)deviceName.header.size);
            Marshal.StructureToPtr(deviceName, intPtr, false);

            var ptr = (DISPLAYCONFIG_DEVICE_INFO_HEADER*)intPtr.ToPointer();

            var success = PInvoke.DisplayConfigGetDeviceInfo(ptr);
            if (success != PInvokeExtensions.ERROR_SUCCESS)
                PInvokeExtensions.ThrowIfWin32Error("DisplayConfigGetDeviceInfo");

            var deviceNameResponse = Marshal.PtrToStructure<DISPLAYCONFIG_TARGET_DEVICE_NAME>(intPtr);
            return deviceNameResponse.outputTechnology;
        }
        catch
        {
            return DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER;
        }
        finally
        {
            Marshal.FreeHGlobal(intPtr);
        }
    }

    private static bool IsInternalOutput(this DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology)
    {
        var result = outputTechnology is DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL;
        result |= outputTechnology is DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED;
        return result;
    }

    private static bool IsExternalDisplayPortOutput(this DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology)
    {
        return outputTechnology is DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL;
    }
}