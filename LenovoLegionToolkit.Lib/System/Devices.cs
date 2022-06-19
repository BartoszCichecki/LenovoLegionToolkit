using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.System
{
    internal static class Devices
    {
        private static readonly object _locker = new();

        private static SafeFileHandle? _battery;

        public static SafeFileHandle GetBattery()
        {
            if (_battery is null)
            {
                lock (_locker)
                {
                    if (_battery is null)
                    {
                        var devClassBatteryGuid = Native.GUID_DEVCLASS_BATTERY;
                        var deviceHandle = Native.SetupDiGetClassDevs(ref devClassBatteryGuid, null, IntPtr.Zero, DeviceGetClassFlagsEx.DIGCF_PRESENT | DeviceGetClassFlagsEx.DIGCF_DEVICEINTERFACE);
                        if (deviceHandle == IntPtr.Zero)
                            NativeUtils.ThrowIfWin32Error("SetupDiGetClassDevs");

                        var deviceInterfaceData = new SpDeviceInterfaceDataEx { CbSize = Marshal.SizeOf<SpDeviceInterfaceDataEx>() };
                        var deviceDetailData = new SpDeviceInterfaceDetailDataEx { CbSize = (IntPtr.Size == 8) ? 8 : 4 + Marshal.SystemDefaultCharSize };

                        var result1 = Native.SetupDiEnumDeviceInterfaces(deviceHandle, IntPtr.Zero, ref devClassBatteryGuid, 0, ref deviceInterfaceData);
                        if (!result1)
                            NativeUtils.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

                        var result2 = Native.SetupDiGetDeviceInterfaceDetail(deviceHandle, ref deviceInterfaceData, ref deviceDetailData, Native.DEVICE_INTERFACE_BUFFER_SIZE, out _, IntPtr.Zero);
                        if (!result2)
                            NativeUtils.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

                        var fileHandle = Native.CreateFile(deviceDetailData.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributesEx.Normal, IntPtr.Zero);
                        if (fileHandle == IntPtr.Zero)
                            NativeUtils.ThrowIfWin32Error("CreateFile");

                        _battery = new SafeFileHandle(fileHandle, true);
                    }
                }
            }
            return _battery;
        }
    }
}
