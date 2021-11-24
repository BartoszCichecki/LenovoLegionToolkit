using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

#pragma warning disable CA1401 // P/Invokes should not be visible
#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments

namespace LenovoLegionToolkit.Lib.Utils
{
    public enum FirmwareType
    {
        Unknown,
        Bios,
        Uefi,
        Max,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TokenPrivelege
    {
        public int Count;
        public long Luid;
        public int Attr;
    }

    public static class Native
    {
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
          string lpSystemName,
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
            string lpBinaryPathName,
            String lpLoadOrderGroup,
            IntPtr lpdwTagId,
            [In] char[] lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        public static extern int CloseServiceHandle(IntPtr hSCObject);
    }
}