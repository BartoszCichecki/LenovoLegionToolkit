using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class IGPUModeFeature : AbstractLenovoGamezoneWmiFeature<IGPUModeState>
    {
        public IGPUModeFeature() : base("IGPUModeStatus", 0, "IsSupportIGPUMode", inParameterName: "mode") { }

        public async Task NotifyAsync()
        {
            try
            {
                if (!await IsSupportedAsync().ConfigureAwait(false))
                    return;

                var isAvailable = await IsDGPUAvailableAsync().ConfigureAwait(false);
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
                        var id = pdc["Data"].ToString();
                        return HardwareId.FromDGPUHWId(id);
                    }).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return new();
            }
        }

        private async Task<bool> IsDGPUAvailableAsync()
        {
            var dgpuHardwareId = await GetDGPUHardwareId().ConfigureAwait(false);

            var guidDisplayDeviceArrival = Native.GUID_DISPLAY_DEVICE_ARRIVAL;
            var deviceHandle = Native.SetupDiGetClassDevs(ref guidDisplayDeviceArrival,
                null,
                IntPtr.Zero,
                DeviceGetClassFlagsEx.DIGCF_DEVICEINTERFACE | DeviceGetClassFlagsEx.DIGCF_PRESENT | DeviceGetClassFlagsEx.DIGCF_PROFILE);

            uint index = 0;
            while (true)
            {
                var currentIndex = index;
                index++;

                var deviceInfoData = new SpDeviceInfoDataEx { CbSize = Marshal.SizeOf<SpDeviceInfoDataEx>() };
                var result1 = Native.SetupDiEnumDeviceInfo(deviceHandle, currentIndex, ref deviceInfoData);
                if (!result1)
                {
                    if (Marshal.GetLastWin32Error() == Native.ERROR_NO_MORE_ITEMS)
                        break;

                    NativeUtils.ThrowIfWin32Error("SetupDiEnumDeviceInfo");
                }

                var deviceInterfaceData = new SpDeviceInterfaceDataEx { CbSize = Marshal.SizeOf<SpDeviceInterfaceDataEx>() };
                var deviceDetailData = new SpDeviceInterfaceDetailDataEx { CbSize = IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize };

                var result2 = Native.SetupDiEnumDeviceInterfaces(deviceHandle,
                    IntPtr.Zero,
                    ref guidDisplayDeviceArrival,
                    currentIndex,
                    ref deviceInterfaceData);
                if (!result2)
                    NativeUtils.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

                _ = Native.SetupDiGetDeviceInterfaceDetail(deviceHandle,
                    ref deviceInterfaceData,
                    IntPtr.Zero,
                    0,
                    out var deviceDetailDataSize,
                    IntPtr.Zero);

                var result3 = Native.SetupDiGetDeviceInterfaceDetail(deviceHandle,
                    ref deviceInterfaceData,
                    ref deviceDetailData,
                    deviceDetailDataSize,
                    out _,
                    IntPtr.Zero);
                if (!result3)
                    NativeUtils.ThrowIfWin32Error("SetupDiGetDeviceInterfaceDetail");

                if (!deviceDetailData.DevicePath.Contains(guidDisplayDeviceArrival.ToString()))
                    continue;

                if (dgpuHardwareId != HardwareId.FromDevicePath(deviceDetailData.DevicePath))
                    continue;

                if (Native.CM_Get_DevNode_Status(out var status, out _, deviceInfoData.DevInst, 0) != 0)
                    continue;

                if ((status & 0x400) != 0)
                    continue;

                return true;
            }

            return false;
        }
    }
}