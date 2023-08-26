using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Security;

namespace LenovoLegionToolkit.Lib.System;

public static class BootLogo
{
    private const string LBLDESP = "LBLDESP";
    private const string LBLDESP_GUID = "{871455D0-5576-4FB8-9865-AF0824463B9E}";
    private const string LBLDVC = "LBLDVC";
    private const string LBLDVC_GUID = "{871455D1-5576-4FB8-9865-AF0824463C9F}";

    private const uint SCOPE_ATTR = PInvokeExtensions.VARIABLE_ATTRIBUTE_NON_VOLATILE |
                                    PInvokeExtensions.VARIABLE_ATTRIBUTE_BOOTSERVICE_ACCESS |
                                    PInvokeExtensions.VARIABLE_ATTRIBUTE_RUNTIME_ACCESS;

    public static bool IsSupported()
    {
        try
        {
            _ = GetInfo();
            _ = GetChecksum();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static (bool, Resolution, ImageFormat[], string[]) GetStatus()
    {
        var info = GetInfo();
        return (info.Enabled == 1, new(info.SupportedWidth, info.SupportedHeight), info.SupportedFormat.ImageFormats().ToArray(), info.SupportedFormat.ExtensionFilters().ToArray());
    }

    public static async Task EnableAsync(string sourcePath)
    {
        var info = GetInfo();

        ThrowIfImageInvalid(info, sourcePath);

        await DeleteMyLogoAsync().ConfigureAwait(false);
        var crc = await CopyMyLogoAsync(info, sourcePath).ConfigureAwait(false);

        SetChecksum(crc);
        SetInfo(info with { Enabled = 1 });
    }

    public static async Task DisableAsync()
    {
        await DeleteMyLogoAsync().ConfigureAwait(false);

        SetChecksum(0);
        SetInfo(GetInfo() with { Enabled = 0 });
    }

    private static unsafe BootLogoInfo GetInfo()
    {
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<BootLogoInfo>());

        try
        {
            if (!SetPrivilege(true))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges.");

                throw new InvalidOperationException("Cannot set UEFI privilege.");
            }

            var ptrSize = (uint)Marshal.SizeOf<BootLogoInfo>();

            var size = PInvoke.GetFirmwareEnvironmentVariableEx(LBLDESP, LBLDESP_GUID, ptr.ToPointer(), ptrSize, null);
            if (size != ptrSize)
                PInvokeExtensions.ThrowIfWin32Error("GetFirmwareEnvironmentVariableEx");

            return Marshal.PtrToStructure<BootLogoInfo>(ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);

            SetPrivilege(false);
        }
    }

    private static unsafe void SetInfo(BootLogoInfo bootLogoInfo)
    {
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<BootLogoInfo>());

        try
        {
            if (!SetPrivilege(true))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges.");

                throw new InvalidOperationException("Cannot set UEFI privilege.");
            }

            Marshal.StructureToPtr(bootLogoInfo, ptr, false);
            var ptrSize = (uint)Marshal.SizeOf<BootLogoInfo>();

            if (!PInvoke.SetFirmwareEnvironmentVariableEx(LBLDESP, LBLDESP_GUID, ptr.ToPointer(), ptrSize, SCOPE_ATTR))
                PInvokeExtensions.ThrowIfWin32Error("SetFirmwareEnvironmentVariableEx");
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);

            SetPrivilege(false);
        }
    }

    private static unsafe uint GetChecksum()
    {
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<BootLogoChecksum>());

        try
        {
            if (!SetPrivilege(true))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges.");

                throw new InvalidOperationException("Cannot set UEFI privilege.");
            }

            var ptrSize = (uint)Marshal.SizeOf<BootLogoChecksum>();

            var size = PInvoke.GetFirmwareEnvironmentVariableEx(LBLDVC, LBLDVC_GUID, ptr.ToPointer(), ptrSize, null);
            if (size != ptrSize)
                PInvokeExtensions.ThrowIfWin32Error("GetFirmwareEnvironmentVariableEx");

            return Marshal.PtrToStructure<BootLogoChecksum>(ptr).Crc;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);

            SetPrivilege(false);
        }
    }

    private static unsafe void SetChecksum(uint checksum)
    {
        var str = new BootLogoChecksum { Crc = checksum };
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<BootLogoChecksum>());

        try
        {
            if (!SetPrivilege(true))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges.");

                throw new InvalidOperationException("Cannot set UEFI privilege.");
            }

            Marshal.StructureToPtr(str, ptr, false);
            var ptrSize = (uint)Marshal.SizeOf<BootLogoChecksum>();

            if (!PInvoke.SetFirmwareEnvironmentVariableEx(LBLDVC, LBLDVC_GUID, ptr.ToPointer(), ptrSize, SCOPE_ATTR))
                PInvokeExtensions.ThrowIfWin32Error("SetFirmwareEnvironmentVariableEx");
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);

            SetPrivilege(false);
        }
    }

    private static async Task<uint> CopyMyLogoAsync(BootLogoInfo bootLogoInfo, string sourcePath)
    {
        char? drive = null;

        try
        {
            drive = await MountEfiPartitionAsync().ConfigureAwait(false);
            if (!drive.HasValue)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot mount EFI partition.");

                throw new InvalidOperationException("Cannot mount EFI partition.");
            }

            if (new DriveInfo($"{drive}:").AvailableFreeSpace < new FileInfo(sourcePath).Length)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Not enough free space on EFI partition.");

                throw new InvalidOperationException("Not enough free space on EFI partition.");
            }

            var destinationDirectory = Path.Combine($"{drive}:", "EFI", "Lenovo", "Logo");
            var filename = $"mylogo_{bootLogoInfo.SupportedWidth}x{bootLogoInfo.SupportedHeight}{Path.GetExtension(sourcePath)}";
            var destinationPath = Path.Combine(destinationDirectory, filename);

            Directory.CreateDirectory(destinationDirectory);
            File.Copy(sourcePath, destinationPath, true);

            return Crc32Adler.Calculate(destinationPath);
        }
        finally
        {
            if (drive.HasValue)
                await UnMountEfiPartitionAsync(drive.Value).ConfigureAwait(false);
        }
    }

    private static async Task DeleteMyLogoAsync()
    {
        char? drive = null;

        try
        {
            drive = await MountEfiPartitionAsync().ConfigureAwait(false);
            if (!drive.HasValue)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot mount EFI partition");

                throw new InvalidOperationException("Cannot mount EFI partition.");
            }

            var directoryPath = $@"{drive}:\EFI\Lenovo\Logo";
            if (!Directory.Exists(directoryPath))
                return;

            Directory.EnumerateFiles(directoryPath, "mylogo*").ForEach(File.Delete);
        }
        finally
        {
            if (drive.HasValue)
                await UnMountEfiPartitionAsync(drive.Value).ConfigureAwait(false);
        }
    }

    private static void ThrowIfImageInvalid(BootLogoInfo info, string sourcePath)
    {
        using var image = Image.FromFile(sourcePath);

        if (info.SupportedWidth != image.Width || info.SupportedHeight != image.Height)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Invalid image size.");

            throw new InvalidOperationException("Invalid image size.");
        }

        if (!info.SupportedFormat.ImageFormats().Contains(image.RawFormat))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Invalid image format.");

            throw new InvalidOperationException("Invalid image format.");
        }
    }

    private static char GetUnusedDriveLetter()
    {
        // ReSharper disable once StringLiteralTypo
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        var usedLetters = DriveInfo.GetDrives().Select(di => di.Name.First()).ToArray();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Used drive letters: {string.Join(",", usedLetters)}");

        var letter = letters.Last(c => !usedLetters.Contains(c));

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Using '{letter}' letter.");

        return letter;
    }

    private static async Task<char?> MountEfiPartitionAsync()
    {
        var drive = GetUnusedDriveLetter();
        var (result, _) = await CMD.RunAsync("mountvol", $"{drive}: /s").ConfigureAwait(false);
        return result == 0 ? drive : null;
    }

    private static Task UnMountEfiPartitionAsync(char letter)
    {
        return CMD.RunAsync("mountvol", $"{letter}: /d");
    }

    private static unsafe bool SetPrivilege(bool enable)
    {
        try
        {
            using var handle = PInvoke.GetCurrentProcess_SafeHandle();

            if (!PInvoke.OpenProcessToken(handle, TOKEN_ACCESS_MASK.TOKEN_QUERY | TOKEN_ACCESS_MASK.TOKEN_ADJUST_PRIVILEGES, out var token))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Could not open process token.");

                return false;
            }

            if (!PInvoke.LookupPrivilegeValue(null, "SeSystemEnvironmentPrivilege", out var luid))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Could not look up privilege value.");

                return false;
            }

            var state = new TOKEN_PRIVILEGES { PrivilegeCount = 1 };
            state.Privileges._0 = new LUID_AND_ATTRIBUTES
            {
                Luid = luid,
                Attributes = enable ? TOKEN_PRIVILEGES_ATTRIBUTES.SE_PRIVILEGE_ENABLED : 0
            };

            if (!PInvoke.AdjustTokenPrivileges(token, false, state, 0, null, null))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Could not adjust token privileges.");

                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Exception while setting privilege.", ex);

            return false;
        }
    }
}
