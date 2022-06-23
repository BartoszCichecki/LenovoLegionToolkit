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
        private static SafeFileHandle? _rgbKeyboard;

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

                        var result2 = Native.SetupDiGetDeviceInterfaceDetail(deviceHandle, ref deviceInterfaceData, ref deviceDetailData, 120, out _, IntPtr.Zero);
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

        public static SafeFileHandle? GetRGBKeyboard()
        {
            if (_rgbKeyboard is null)
            {
                lock (_locker)
                {
                    if (_rgbKeyboard is null)
                    {
                        var vendorId = 0x048D;
                        var productId = 0xC955;
                        var productIdMask = 0xFF0F;
                        var descriptorLength = 0x21;

                        Native.HidD_GetHidGuid(out Guid devClassHIDGuid);

                        var deviceHandle = Native.SetupDiGetClassDevs(ref devClassHIDGuid, null, IntPtr.Zero, DeviceGetClassFlagsEx.DIGCF_PRESENT | DeviceGetClassFlagsEx.DIGCF_DEVICEINTERFACE);

                        uint index = 0;
                        while (true)
                        {
                            uint currentIndex = index;
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
                            var deviceDetailData = new SpDeviceInterfaceDetailDataEx { CbSize = (IntPtr.Size == 8) ? 8 : 4 + Marshal.SystemDefaultCharSize };

                            var result2 = Native.SetupDiEnumDeviceInterfaces(deviceHandle, IntPtr.Zero, ref devClassHIDGuid, currentIndex, ref deviceInterfaceData);
                            if (!result2)
                                NativeUtils.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

                            _ = Native.SetupDiGetDeviceInterfaceDetail(deviceHandle, ref deviceInterfaceData, IntPtr.Zero, 0, out uint deviceDetailDataSize, IntPtr.Zero);

                            var result3 = Native.SetupDiGetDeviceInterfaceDetail(deviceHandle, ref deviceInterfaceData, ref deviceDetailData, deviceDetailDataSize, out _, IntPtr.Zero);
                            if (!result3)
                                NativeUtils.ThrowIfWin32Error("SetupDiGetDeviceInterfaceDetail");

                            var fileHandle = Native.CreateFile(deviceDetailData.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributesEx.Normal, IntPtr.Zero);
                            if (fileHandle == IntPtr.Zero)
                                NativeUtils.ThrowIfWin32Error("CreateFile");

                            var hid = new SafeFileHandle(fileHandle, true);

                            var hiddAttributes = new HIDDAttributesEx { CbSize = Marshal.SizeOf<HIDDAttributesEx>() };
                            var result4 = Native.HidD_GetAttributes(hid, ref hiddAttributes);
                            if (!result4)
                                continue;

                            if (hiddAttributes.VendorID == vendorId && (hiddAttributes.ProductID & productIdMask) == (productId & productIdMask))
                            {
                                var preparsedData = IntPtr.Zero;
                                try
                                {
                                    _ = Native.HidD_GetPreparsedData(hid, ref preparsedData);
                                    _ = Native.HidP_GetCaps(preparsedData, out HIDPCapsEx caps);

                                    if (caps.FeatureReportByteLength == descriptorLength)
                                    {
                                        _rgbKeyboard = hid;
                                        break;
                                    }
                                }
                                finally
                                {
                                    _ = Native.HidD_FreePreparsedData(preparsedData);
                                }

                            }
                        }
                    }
                }
            }
            return _rgbKeyboard;
        }
    }
}
