using System;
using System.Runtime.InteropServices;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace LenovoLegionToolkit.Lib.System;

public static class Devices
{
    private static readonly object Lock = new();

    private static SafeFileHandle? _battery;
    private static SafeFileHandle? _rgbKeyboard;
    private static SafeFileHandle? _spectrumRgbKeyboard;

    public static unsafe SafeFileHandle GetBattery(bool forceRefresh = false)
    {
        if (_battery is not null && !forceRefresh)
            return _battery;

        lock (Lock)
        {
            if (_battery is not null && !forceRefresh)
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

                devicePath = new string(deviceDetailData->DevicePath.Value);
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

    public static SafeFileHandle? GetRGBKeyboard(bool forceRefresh = false)
    {
        if (_rgbKeyboard is not null && !forceRefresh)
            return _rgbKeyboard;

        lock (Lock)
        {
            if (_rgbKeyboard is not null && !forceRefresh)
                return _rgbKeyboard;

            const ushort vendorId = 0x048D;
            const ushort productIdMasked = 0xC900;
            const ushort productIdMask = 0xFF00;
            const ushort descriptorLength = 0x21;

            _rgbKeyboard = FindHidDevice(vendorId, productIdMask, productIdMasked, descriptorLength);
        }

        return _rgbKeyboard;
    }

    public static SafeFileHandle? GetSpectrumRGBKeyboard(bool forceRefresh = false)
    {
        if (_spectrumRgbKeyboard is not null && !forceRefresh)
            return _spectrumRgbKeyboard;

        lock (Lock)
        {
            if (_spectrumRgbKeyboard is not null && !forceRefresh)
                return _spectrumRgbKeyboard;

            const ushort vendorId = 0x048D;
            const ushort productIdMasked = 0xC900;
            const ushort productIdMask = 0xFF00;
            const ushort descriptorLength = 0x03C0;

            _spectrumRgbKeyboard = FindHidDevice(vendorId, productIdMask, productIdMasked, descriptorLength);
        }

        return _spectrumRgbKeyboard;
    }

    private static unsafe SafeFileHandle? FindHidDevice(ushort vendorId, ushort productIdMask, ushort productIdMasked, ushort descriptorLength)
    {
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

            var result2 = PInvoke.SetupDiEnumDeviceInterfaces(deviceHandle, null, devClassHidGuid, currentIndex,
                ref deviceInterfaceData);
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

                var result3 = PInvoke.SetupDiGetDeviceInterfaceDetail(deviceHandle, deviceInterfaceData, deviceDetailData,
                    requiredSize, null, null);
                if (!result3)
                    PInvokeExtensions.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

                devicePath = new string(deviceDetailData->DevicePath.Value);
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

                if (hiddAttributes.VendorID == vendorId && (hiddAttributes.ProductID & productIdMask) == productIdMasked && caps.FeatureReportByteLength == descriptorLength)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Found device. [vendorId={hiddAttributes.VendorID:X2}, productId={hiddAttributes.ProductID:X2}, descriptorLength={caps.FeatureReportByteLength}]");

                    return fileHandle;
                }
            }
            finally
            {
                PInvoke.HidD_FreePreparsedData(preParsedData);
            }
        }

        return null;
    }
}