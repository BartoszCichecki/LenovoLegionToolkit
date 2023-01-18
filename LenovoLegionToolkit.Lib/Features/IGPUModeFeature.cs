using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

namespace LenovoLegionToolkit.Lib.Features;

public class IGPUModeFeature : AbstractLenovoGamezoneWmiFeature<IGPUModeState>
{
    public IGPUModeFeature() : base("IGPUModeStatus", 0, "IsSupportIGPUMode", inParameterName: "mode") { }

    public async Task NotifyAsync()
    {
        try
        {
            var dgpuHardwareId = await GetDGPUHardwareId().ConfigureAwait(false);
            var isAvailable = IsDGPUAvailable(dgpuHardwareId);
            await NotifyDGPUStatusAsync(isAvailable).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Notified: {isAvailable}");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to notify.", ex);
        }
    }

    private Task NotifyDGPUStatusAsync(bool state) => WMI.CallAsync(Scope,
        Query,
        "NotifyDGPUStatus",
        new() { { "Status", state ? "1" : "0" } });

    private async Task<HardwareId> GetDGPUHardwareId()
    {
        try
        {
            return await WMI.CallAsync(Scope,
                Query,
                "GetDGPUHWId",
                new(),
                pdc =>
                {
                    var id = pdc["Data"].Value.ToString();
                    return HardwareIdFromDGPUHWId(id);
                }).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return new();
        }
    }

    private unsafe bool IsDGPUAvailable(HardwareId dgpuHardwareId)
    {
        var guidDisplayDeviceArrival = PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL;
        var deviceHandle = PInvoke.SetupDiGetClassDevs(guidDisplayDeviceArrival,
            null,
            HWND.Null,
            PInvoke.DIGCF_DEVICEINTERFACE | PInvoke.DIGCF_PRESENT | PInvoke.DIGCF_PROFILE);

        uint index = 0;
        while (true)
        {
            var currentIndex = index;
            index++;

            var deviceInfoData = new SP_DEVINFO_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVINFO_DATA>() };
            var result1 = PInvoke.SetupDiEnumDeviceInfo(deviceHandle, currentIndex, ref deviceInfoData);
            if (!result1)
            {
                if (Marshal.GetLastWin32Error() == PInvokeExtensions.ERROR_NO_MORE_ITEMS)
                    break;

                PInvokeExtensions.ThrowIfWin32Error("SetupDiEnumDeviceInfo");
            }

            var deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>() };
            var result2 = PInvoke.SetupDiEnumDeviceInterfaces(deviceHandle, null, guidDisplayDeviceArrival, currentIndex, ref deviceInterfaceData);
            if (!result2)
                PInvokeExtensions.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

            var requiredSize = 0u;
            _ = PInvoke.SetupDiGetDeviceInterfaceDetail(deviceHandle, deviceInterfaceData, null, 0, &requiredSize, null);

            string devicePath;
            var output = IntPtr.Zero;
            try
            {
                output = Marshal.AllocHGlobal((int)requiredSize);
                var deviceDetailData = (SP_DEVICE_INTERFACE_DETAIL_DATA_W*)output.ToPointer();
                deviceDetailData->cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DETAIL_DATA_W>();

                var result3 = PInvoke.SetupDiGetDeviceInterfaceDetail(deviceHandle, deviceInterfaceData, deviceDetailData, requiredSize, null, null);
                if (!result3)
                    PInvokeExtensions.ThrowIfWin32Error("SetupDiGetDeviceInterfaceDetail");

                devicePath = new string(deviceDetailData->DevicePath.Value);
            }
            finally
            {
                Marshal.FreeHGlobal(output);
            }

            if (!devicePath.Contains(guidDisplayDeviceArrival.ToString()))
                continue;

            if (dgpuHardwareId != HardwareIdFromDevicePath(devicePath))
                continue;

            if (PInvoke.CM_Get_DevNode_Status(out var status, out _, deviceInfoData.DevInst, 0) != 0)
                continue;

            if ((status & 0x400) != 0)
                continue;

            return true;
        }

        return false;
    }

    private static HardwareId HardwareIdFromDGPUHWId(string? gpuHwId)
    {
        try
        {
            if (gpuHwId is null)
                return default;

            var matches = new Regex("PCIVEN_([0-9A-F]{4})|DEV_([0-9A-F]{4})|SUBSYS_([0-9A-F]*)").Matches(gpuHwId);
            if (matches.Count != 3)
                return default;

            var vendor = matches[0].Groups[1].Value;
            var device = matches[1].Groups[2].Value;
            var subsystem = matches[2].Groups[3].Value;

            return new HardwareId { Vendor = vendor, Device = device, SubSystem = subsystem };
        }
        catch
        {
            return default;
        }
    }

    private static HardwareId HardwareIdFromDevicePath(string devicePath)
    {
        try
        {
            var matches = new Regex("pci#ven_([0-9A-Fa-f]{4})|dev_([0-9A-Fa-f]{4})|subsys_([0-9A-Fa-f]*)").Matches(devicePath);
            if (matches.Count != 3)
                return default;

            var vendor = matches[0].Groups[1].Value;
            var device = matches[1].Groups[2].Value;
            var subsystem = matches[2].Groups[3].Value;

            return new HardwareId { Vendor = vendor, Device = device, SubSystem = subsystem };
        }
        catch
        {
            return default;
        }
    }
}