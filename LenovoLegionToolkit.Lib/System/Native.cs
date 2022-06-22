using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
#pragma warning disable IDE0044 // Add readonly modifier

namespace LenovoLegionToolkit.Lib.System
{
    internal enum FirmwareTypeEx
    {
        Unknown,
        Bios,
        Uefi,
        Max,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TokenPrivelegeEx
    {
        public int Count;
        public long Luid;
        public int Attr;
    }

    internal enum ACLineStatusEx : byte
    {
        Offline = 0,
        Online = 1,
        Unknown = 255
    }

    internal enum BatteryFlagEx : byte
    {
        High = 1,
        Low = 2,
        Critical = 4,
        Charging = 8,
        NoSystemBattery = 128,
        Unknown = 255
    }

    internal enum BatteryQueryInformationLevelEx
    {
        BatteryInformation = 0,
        BatteryGranularityInformation = 1,
        BatteryTemperature = 2,
        BatteryEstimatedTime = 3,
        BatteryDeviceName = 4,
        BatteryManufactureDate = 5,
        BatteryManufactureName = 6,
        BatteryUniqueID = 7
    }

    [Flags]
    internal enum DeviceGetClassFlagsEx : uint
    {
        DIGCF_DEFAULT = 0x00000001,
        DIGCF_PRESENT = 0x00000002,
        DIGCF_ALLCLASSES = 0x00000004,
        DIGCF_PROFILE = 0x00000008,
        DIGCF_DEVICEINTERFACE = 0x00000010
    }

    [Flags]
    internal enum FileAttributesEx : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Directory = 0x00000010,
        Archive = 0x00000020,
        Device = 0x00000040,
        Normal = 0x00000080,
        Temporary = 0x00000100,
        SparseFile = 0x00000200,
        ReparsePoint = 0x00000400,
        Compressed = 0x00000800,
        Offline = 0x00001000,
        NotContentIndexed = 0x00002000,
        Encrypted = 0x00004000,
        Write_Through = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x08000000,
        DeleteOnClose = 0x04000000,
        BackupSemantics = 0x02000000,
        PosixSemantics = 0x01000000,
        OpenReparsePoint = 0x00200000,
        OpenNoRecall = 0x00100000,
        FirstPipeInstance = 0x00080000
    }

    [Flags]
    internal enum PowerStateEx : uint
    {
        BATTERY_POWER_ONLINE = 0x00000001,
        BATTERY_DISCHARGING = 0x00000002,
        BATTERY_CHARGING = 0x00000004,
        BATTERY_CRITICAL = 0x00000008
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BatteryInformationEx
    {
        public int Capabilities;
        public byte Technology;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Chemistry;
        public int DesignedCapacity;
        public int FullChargedCapacity;
        public int DefaultAlert1;
        public int DefaultAlert2;
        public int CriticalBias;
        public int CycleCount;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BatteryStatusEx
    {
        public PowerStateEx PowerState;
        public uint Capacity;
        public uint Voltage;
        public int Rate;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BatteryQueryInformationEx
    {
        public uint BatteryTag;
        public BatteryQueryInformationLevelEx InformationLevel;
        public int AtRate;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BatteryWaitStatusEx
    {
        public uint BatteryTag;
        public uint Timeout;
        public PowerStateEx PowerState;
        public uint LowCapacity;
        public uint HighCapacity;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LenovoBatteryInformationEx
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        private byte[] bytes1;
        public ushort Temperature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
        private byte[] bytes2;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SpDeviceInfoDataEx
    {
        public int CbSize;
        public Guid InterfaceClassGuid;
        public int Flags;
        public UIntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SpDeviceInterfaceDataEx
    {
        public int CbSize;
        public Guid InterfaceClassGuid;
        public int Flags;
        public UIntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SpDeviceInterfaceDetailDataEx
    {
        public int CbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DevicePath;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SystemPowerStatusEx
    {
        public ACLineStatusEx ACLineStatus;
        public BatteryFlagEx BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HIDDAttributesEx
    {
        public int CbSize;
        public ushort VendorID;
        public ushort ProductID;
        public ushort VersionNumber;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HIDPCapsEx
    {
        public ushort Usage;
        public ushort UsagePage;
        public ushort InputReportByteLength;
        public ushort OutputReportByteLength;
        public ushort FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public ushort[] Reserved;
        public ushort NumberLinkCollectionNodes;
        public ushort NumberInputButtonCaps;
        public ushort NumberInputValueCaps;
        public ushort NumberInputDataIndices;
        public ushort NumberOutputButtonCaps;
        public ushort NumberOutputValueCaps;
        public ushort NumberOutputDataIndices;
        public ushort NumberFeatureButtonCaps;
        public ushort NumberFeatureValueCaps;
        public ushort NumberFeatureDataIndices;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBKeyboardStateEx
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Header;
        public byte Effect;
        public byte Speed;
        public byte Brightness;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Zone1Rgb;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Zone2Rgb;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Zone3Rgb;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Zone4Rgb;
        public byte Padding;
        public byte WaveLTR;
        public byte WaveRTL;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] Unused;
    }

    internal static class Native
    {
        public static readonly Guid GUID_DEVCLASS_BATTERY = new(0x72631E54, 0x78A4, 0x11D0, 0xBC, 0xF7, 0x00, 0xAA, 0x00, 0xB7, 0xB3, 0x2A);

        public const uint IOCTL_BATTERY_QUERY_TAG = (0x00000029 << 16) | ((int)FileAccess.Read << 14) | (0x10 << 2) | (0);
        public const uint IOCTL_BATTERY_QUERY_INFORMATION = (0x00000029 << 16) | ((int)FileAccess.Read << 14) | (0x11 << 2) | (0);
        public const uint IOCTL_BATTERY_QUERY_STATUS = (0x00000029 << 16) | ((int)FileAccess.Read << 14) | (0x13 << 2) | (0);

        public const uint DIGCF_PRESENT = 0x2;
        public const uint DIGCF_DEVICEINTERFACE = 0x10;

        public const int ERROR_NO_MORE_ITEMS = 259;

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(
            ref Guid guid,
            string? enumerator,
            IntPtr hwnd,
            DeviceGetClassFlagsEx flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr hdevInfo,
            IntPtr devInfo,
            ref Guid guid,
            uint memberIndex,
            ref SpDeviceInterfaceDataEx devInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(
            IntPtr hdevInfo,
            uint index,
            ref SpDeviceInfoDataEx deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr hdevInfo,
            ref SpDeviceInterfaceDataEx deviceInterfaceData,
            ref SpDeviceInterfaceDetailDataEx deviceInterfaceDetailData,
            uint deviceInterfaceDetailDataSize,
            out uint requiredSize,
            IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr hdevInfo,
            ref SpDeviceInterfaceDataEx deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            uint deviceInterfaceDetailDataSize,
            out uint requiredSize,
            IntPtr deviceInfoData);

        [DllImport("kernel32.dll")]
        public static extern bool GetSystemPowerStatus(out SystemPowerStatusEx sps);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
            string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess desiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare shareMode,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributesEx flags,
            IntPtr template);

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
            ref uint inBuffer,
            int nInBufferSize,
            out uint outBuffer,
            int nOutBufferSize,
            out int pBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref uint inBuffer,
            int nInBufferSize,
            [Out] IntPtr outBuffer,
            int nOutBufferSize,
            out int pBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            [In] IntPtr inBuffer,
            int nInBufferSize,
            [Out] IntPtr outBuffer,
            int nOutBufferSize,
            out uint pBytesReturned,
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
        public static extern bool GetFirmwareType(ref FirmwareTypeEx firmwareType);

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
          ref TokenPrivelegeEx newState,
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

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string? machineName, string? databaseName, uint dwAccess);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        public static extern int CloseServiceHandle(IntPtr hSCObject);

        [DllImport("hid.dll", EntryPoint = "HidD_GetHidGuid", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void HidD_GetHidGuid(out Guid guid);

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_GetAttributes(SafeFileHandle handle, ref HIDDAttributesEx attributes);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_GetPreparsedData(SafeFileHandle handle, ref IntPtr preparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern uint HidP_GetCaps(IntPtr preparsedData, out HIDPCapsEx capabilities);
        
        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_SetFeature(SafeFileHandle handle, ref byte[] buffer, uint bufferLength);
    }

    internal static class NativeUtils
    {
        public static void ThrowIfWin32Error(string description)
        {
            int errorCode = Marshal.GetLastWin32Error();
            if (errorCode != 0)
                throw Marshal.GetExceptionForHR(errorCode) ?? throw new Exception($"Unknown Win32 error code {errorCode} in {description}.");
            else
                throw new Exception($"{description} failed but Win32 didn't catch an error.");
        }
    }
}
