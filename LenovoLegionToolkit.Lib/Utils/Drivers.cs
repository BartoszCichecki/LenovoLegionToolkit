using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal static class Drivers
    {
        private static readonly object _locker = new();

        private static SafeFileHandle? _energy;
        private static SafeFileHandle? _hid;
        public static SafeFileHandle GetEnergy()
        {
            if (_energy == null)
            {
                lock (_locker)
                {
                    if (_energy == null)
                    {
                        var fileHandle = Native.CreateFileW("\\\\.\\EnergyDrv", 0xC0000000, 3u, IntPtr.Zero, 3u, 0x80, IntPtr.Zero);
                        if (fileHandle == new IntPtr(-1))
                            throw new InvalidOperationException("fileHandle is 0");
                        _energy = new SafeFileHandle(fileHandle, true);
                    }
                }
            }
            return _energy;
        }


        public static Boolean IsRGBKeyboardSupported()
        {
            return (LocateHid(0x048D, 0xC955, 0xFF0F, 33) != null);
        }

        public static SafeFileHandle GetHid()
        {
            if (_hid == null)
            {
                _hid = LocateHid(0x048D, 0xC955, 0xFF0F, 33);
                if (_hid == null)
                    throw new InvalidOperationException("fileHandle is 0");

            }
            return _hid;
        }

        public static SafeFileHandle? LocateHid(ushort VendorId, ushort ProductId, ushort ProductIdMask, ushort DescriptorLenght)
        {
            SafeFileHandle? hid = null;
            lock (_locker)
            {
                //TODO Find a Better Way to locate the HID
                Guid HIDClassGuid;
                Native.HidD_GetHidGuid(out HIDClassGuid);
                // Todo Define DIGCF_PRESENT | DIGCF_DEVICEINTERFACE
                IntPtr DeviceInfoSet = Native.SetupDiGetClassDevs(ref HIDClassGuid, null, IntPtr.Zero, 0x12);
                SP_DEVINFO_DATA DeviceInfoData = new SP_DEVINFO_DATA();
                SP_DEVICE_INTERFACE_DATA deviceData = new SP_DEVICE_INTERFACE_DATA();
                HIDD_ATTRIBUTES DeviceAttributes = new HIDD_ATTRIBUTES();
                SP_DEVICE_INTERFACE_DETAIL_DATA sP_DEVICE_INTERFACE_DETAIL_DATA = new SP_DEVICE_INTERFACE_DETAIL_DATA();

                deviceData.cbSize = Marshal.SizeOf(deviceData);
                DeviceInfoData.cbSize = (uint)Marshal.SizeOf(DeviceInfoData);
                UInt16 index = 0;
                UInt32 interfaceInfoSize = 0;
                Boolean Found = false;
                Boolean result = false;
                //TODO add Check on Malloc etc...
                //TODO explicit Call the W variant to Avoid Error
                while (!Found && Native.SetupDiEnumDeviceInfo(DeviceInfoSet, index, ref DeviceInfoData))
                {
                    result = Native.SetupDiEnumDeviceInterfaces(DeviceInfoSet, IntPtr.Zero, ref HIDClassGuid, index, ref deviceData);
                    result = Native.SetupDiGetDeviceInterfaceDetail(DeviceInfoSet, ref deviceData, IntPtr.Zero, 0, out interfaceInfoSize, IntPtr.Zero);

                    IntPtr deviceInterfaceDetail = Marshal.AllocHGlobal((Int32)interfaceInfoSize);
                    sP_DEVICE_INTERFACE_DETAIL_DATA.size = Marshal.SizeOf(sP_DEVICE_INTERFACE_DETAIL_DATA);
                    Marshal.StructureToPtr(sP_DEVICE_INTERFACE_DETAIL_DATA, deviceInterfaceDetail, false);


                    if (Native.SetupDiGetDeviceInterfaceDetail(DeviceInfoSet, ref deviceData, deviceInterfaceDetail, interfaceInfoSize, IntPtr.Zero, IntPtr.Zero))
                    {
                        String DevicePath = Marshal.PtrToStringUni(deviceInterfaceDetail + 4);

                        var fileHandle = Native.CreateFileW(DevicePath, 0xC0000000, 3u, IntPtr.Zero, 3u, 0x80, IntPtr.Zero);
                        if (fileHandle != new IntPtr(-1))
                        {
                            hid = new SafeFileHandle(fileHandle, true);
                             if (Native.HidD_GetAttributes(hid, ref DeviceAttributes))
                             {
                                 if (DeviceAttributes.VendorID == 0x048D && (DeviceAttributes.ProductID & ProductIdMask) == (ProductId & ProductIdMask))
                                 {
                                     IntPtr PreparsedData = IntPtr.Zero;
                                     HIDP_CAPS Caps = new HIDP_CAPS();
                                     Native.HidD_GetPreparsedData(hid, ref PreparsedData);


                                     Native.HidP_GetCaps(PreparsedData, out Caps);
                                     Native.HidD_FreePreparsedData(PreparsedData);
                                     if (Caps.FeatureReportByteLength == DescriptorLenght)
                                     {
                                         Found = true;
                                         break;
                                     }
                                 }


                             }
                            hid = null;
                        }
                    }
                    else
                    {
                        int error = Marshal.GetLastWin32Error();
                    }

                    Marshal.FreeHGlobal(deviceInterfaceDetail);
                    index++;
                }
                Native.SetupDiDestroyDeviceInfoList(DeviceInfoSet);
            }

            return hid;
        }
    }
}
