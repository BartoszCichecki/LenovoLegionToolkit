using System;
using System.Runtime.InteropServices;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace LenovoLegionToolkit.Lib.System
{
    internal static class Devices
    {
        private static readonly object _locker = new();

        private static SafeFileHandle? _battery;
        private static SafeFileHandle? _rgbKeyboard;

        public static unsafe SafeFileHandle GetBattery()
        {
            if (_battery is not null)
                return _battery;

            lock (_locker)
            {
                if (_battery is not null)
                    return _battery;

                var devClassBatteryGuid = PInvoke.GUID_DEVCLASS_BATTERY;
                var deviceHandle = PInvoke.SetupDiGetClassDevs(devClassBatteryGuid,
                    null,
                    HWND.Null,
                    PInvoke.DIGCF_PRESENT | PInvoke.DIGCF_DEVICEINTERFACE);
                if (deviceHandle.IsInvalid)
                    PInvokeExtensions.ThrowIfWin32Error("SetupDiGetClassDevs");

                var deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>() };

                var result1 = PInvoke.SetupDiEnumDeviceInterfaces(deviceHandle, null, devClassBatteryGuid, 0, ref deviceInterfaceData);
                if (!result1)
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
                        PInvokeExtensions.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

                    devicePath = new string(&deviceDetailData->DevicePath._0);
                }
                finally
                {
                    Marshal.FreeHGlobal(output);
                }

                var fileHandle = PInvoke.CreateFile(devicePath,
                    FILE_ACCESS_FLAGS.FILE_READ_DATA | FILE_ACCESS_FLAGS.FILE_WRITE_DATA,
                    FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                    null,
                    FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                    FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                    null);

                if (fileHandle.IsInvalid)
                    PInvokeExtensions.ThrowIfWin32Error("CreateFile");

                _battery = fileHandle;
            }

            return _battery;
        }

        public static unsafe SafeFileHandle? GetRGBKeyboard()
        {
            if (_rgbKeyboard is not null)
                return _rgbKeyboard;

            lock (_locker)
            {
                if (_rgbKeyboard is not null)
                    return _rgbKeyboard;

                var vendorId = 0x048D;
                var productIdMasked = 0xC900;
                var productIdMask = 0xFF00;
                var descriptorLength = 0x21;

                PInvoke.HidD_GetHidGuid(out var devClassHidGuid);

                var deviceHandle = PInvoke.SetupDiGetClassDevs(devClassHidGuid,
                    null,
                    HWND.Null,
                    PInvoke.DIGCF_PRESENT | PInvoke.DIGCF_DEVICEINTERFACE);

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

                    var result2 = PInvoke.SetupDiEnumDeviceInterfaces(deviceHandle, null, devClassHidGuid, currentIndex, ref deviceInterfaceData);
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
                            PInvokeExtensions.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

                        devicePath = new string(&deviceDetailData->DevicePath._0);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(output);
                    }

                    var fileHandle = PInvoke.CreateFile(devicePath,
                        FILE_ACCESS_FLAGS.FILE_READ_DATA | FILE_ACCESS_FLAGS.FILE_WRITE_DATA,
                        FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                        null,
                        FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                        FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                        null);

                    if (!PInvoke.HidD_GetAttributes(fileHandle, out var hiddAttributes))
                        continue;

                    nint preParsedData = 0;
                    try
                    {
                        PInvoke.HidD_GetPreparsedData(fileHandle, out preParsedData);
                        PInvoke.HidP_GetCaps(preParsedData, out var caps);

                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Checking device... [vendorId={hiddAttributes.VendorID:X2}, productId={hiddAttributes.ProductID:X2}, descriptorLength={caps.FeatureReportByteLength}]");

                        if (hiddAttributes.VendorID == vendorId && (hiddAttributes.ProductID & productIdMask) == productIdMasked && caps.FeatureReportByteLength == descriptorLength)
                        {
                            _rgbKeyboard = fileHandle;
                            break;
                        }
                    }
                    finally
                    {
                        PInvoke.HidD_FreePreparsedData(preParsedData);
                    }
                }
            }

            return _rgbKeyboard;
        }
    }
}
