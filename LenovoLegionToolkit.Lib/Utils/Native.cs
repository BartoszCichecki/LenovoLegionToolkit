using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments

namespace LenovoLegionToolkit.Lib.Utils
{
    internal enum FirmwareType
    {
        Unknown,
        Bios,
        Uefi,
        Max,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TokenPrivelege
    {
        public int Count;
        public long Luid;
        public int Attr;
    }

    internal enum ACLineStatus : byte
    {
        Offline = 0,
        Online = 1,
        Unknown = 255
    }

    internal enum BatteryFlag : byte
    {
        High = 1,
        Low = 2,
        Critical = 4,
        Charging = 8,
        NoSystemBattery = 128,
        Unknown = 255
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SystemPowerStatus
    {
        public ACLineStatus ACLineStatus;
        public BatteryFlag BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVINFO_DATA
    {
        public UInt32 cbSize;
        public Guid ClassGuid;
        public UInt32 DevInst;
        public IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVICE_INTERFACE_DATA
    {
        public Int32 cbSize;
        public Guid interfaceClassGuid;
        public Int32 flags;
        private UIntPtr reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        public int size;
        public int devicePath;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HIDD_ATTRIBUTES
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint Size;

        [MarshalAs(UnmanagedType.U2)]
        public ushort VendorID;

        [MarshalAs(UnmanagedType.U2)]
        public ushort ProductID;

        [MarshalAs(UnmanagedType.U2)]
        public ushort VersionNumber;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HIDP_CAPS
    {
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 Usage;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 UsagePage;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 InputReportByteLength;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 OutputReportByteLength;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public UInt16[] Reserved;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberLinkCollectionNodes;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberInputButtonCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberInputValueCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberInputDataIndices;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberOutputButtonCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberOutputValueCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberOutputDataIndices;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberFeatureButtonCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberFeatureValueCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberFeatureDataIndices;
    };



    internal static class Native
    {
        [DllImport("Kernel32")]
        public static extern bool GetSystemPowerStatus(out SystemPowerStatus sps);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string filename,
            uint access,
            uint share,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref byte InBuffer,
            int nInBufferSize,
            out uint OutBuffer,
            int nOutBufferSize,
            out int pBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetFirmwareEnvironmentVariableExW(
          [MarshalAs(UnmanagedType.LPWStr)] string lpName,
          [MarshalAs(UnmanagedType.LPWStr)] string lpGuid,
          IntPtr pBuffer,
          int nSize,
          IntPtr pAttribute);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetFirmwareEnvironmentVariableExW(
          [MarshalAs(UnmanagedType.LPWStr)] string lpName,
          [MarshalAs(UnmanagedType.LPWStr)] string lpGuid,
          IntPtr pBuffer,
          int nSize,
          int attribute);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFirmwareType(ref FirmwareType firmwareType);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeValue(
          string? lpSystemName,
          string lpName,
          ref long lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(
          IntPtr tokenHandle,
          [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges,
          ref TokenPrivelege newState,
          int zero,
          IntPtr null1,
          IntPtr null2);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
          IntPtr processHandle,
          uint desiredAccess,
          ref IntPtr tokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ChangeServiceConfig(
            IntPtr hService,
            uint nServiceType,
            uint nStartType,
            uint nErrorControl,
            string? lpBinaryPathName,
            string? lpLoadOrderGroup,
            IntPtr lpdwTagId,
            [In] char[]? lpDependencies,
            string? lpServiceStartName,
            string? lpPassword,
            string? lpDisplayName);


        [DllImport("hid.dll", EntryPoint = "HidD_GetHidGuid", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void HidD_GetHidGuid(out Guid Guid);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(
                                              ref Guid ClassGuid,
                                              [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
                                              IntPtr hwndParent,
                                              uint Flags
                                             );

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);


        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(
                                                IntPtr hDevInfo,
                                                IntPtr devInfo,
                                                ref Guid interfaceClassGuid,
                                                UInt32 memberIndex,
                                                ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
                                            );


        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(
           IntPtr hDevInfo,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
           IntPtr deviceInterfaceDetailData,
           UInt32 deviceInterfaceDetailDataSize,
           out UInt32 requiredSize,
           IntPtr deviceInfoData
        );

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(
                                       IntPtr hDevInfo,
                                       ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                                       IntPtr deviceInterfaceDetailData,
                                       UInt32 deviceInterfaceDetailDataSize,
                                       IntPtr requiredSize,
                                       IntPtr deviceInfoData
                                    );

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_GetAttributes(
                                        SafeFileHandle HidDeviceObject,
                                        ref HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_GetPreparsedData(SafeFileHandle HidDeviceObject, ref IntPtr PreparsedData);


        [DllImport("hid.dll", SetLastError = true)]
        public static extern uint HidP_GetCaps(
                                        IntPtr PreparsedData,
                                        out HIDP_CAPS Capabilities
                                    );

        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_FreePreparsedData(IntPtr PreparsedData);


        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_SetFeature(SafeFileHandle HidDeviceObject, ref LegionRGBKey Buffer, uint BufferLength);


        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList (
                                                                 IntPtr DeviceInfoSet
                                                            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string? machineName, string? databaseName, uint dwAccess);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        public static extern int CloseServiceHandle(IntPtr hSCObject);
    }
}