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

namespace LenovoLegionToolkit.Lib.System;

public class CantSetUEFIPrivilegeException : Exception;

public class CantMountUEFIPartitionException : Exception;

public class NotEnoughSpaceOnUEFIPartitionException : Exception;

public class InvalidBootLogoImageFormatException : Exception;

public class InvalidBootLogoImageSizeException : Exception;

public static class BootLogo
{
    private const string LBLDESP = "LBLDESP";
    private const string LBLDESP_GUID = "{871455D0-5576-4FB8-9865-AF0824463B9E}";
    private const string LBLDVC = "LBLDVC";
    private const string LBLDVC_GUID = "{871455D1-5576-4FB8-9865-AF0824463C9F}";

    private const uint SCOPE_ATTR = PInvokeExtensions.VARIABLE_ATTRIBUTE_NON_VOLATILE |
                                    PInvokeExtensions.VARIABLE_ATTRIBUTE_BOOTSERVICE_ACCESS |
                                    PInvokeExtensions.VARIABLE_ATTRIBUTE_RUNTIME_ACCESS;

    public static async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            if (!mi.Properties.SupportBootLogoChange)
                return false;

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
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Enabling logo... [sourcePath={sourcePath}]");

        var info = GetInfo();

        ThrowIfImageInvalid(info, sourcePath);

        await DeleteMyLogoAsync().ConfigureAwait(false);
        var crc = await CopyMyLogoAsync(info, sourcePath).ConfigureAwait(false);

        SetChecksum(crc);
        SetInfo(info with { Enabled = 1 });

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Enabled logo. [sourcePath={sourcePath}]");
    }

    public static async Task DisableAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Disabling logo...");

        await DeleteMyLogoAsync().ConfigureAwait(false);

        SetChecksum(0);
        SetInfo(GetInfo() with { Enabled = 0 });

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Disabled logo.");
    }

    private static unsafe BootLogoInfo GetInfo()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting info...");

        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<BootLogoInfo>());

        try
        {
            if (!TokenManipulator.AddPrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges.");

                throw new CantSetUEFIPrivilegeException();
            }

            var ptrSize = (uint)Marshal.SizeOf<BootLogoInfo>();

            var size = PInvoke.GetFirmwareEnvironmentVariableEx(LBLDESP, LBLDESP_GUID, ptr.ToPointer(), ptrSize, null);
            if (size != ptrSize)
                PInvokeExtensions.ThrowIfWin32Error("GetFirmwareEnvironmentVariableEx");

            var str = Marshal.PtrToStructure<BootLogoInfo>(ptr);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Retrieved info. [enabled={str.Enabled}, supportedWidth={str.SupportedWidth}, supportedHeight={str.SupportedHeight}, supportedFormat={(int)str.SupportedFormat}]");

            return str;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);

            TokenManipulator.RemovePrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE);
        }
    }

    private static unsafe void SetInfo(BootLogoInfo info)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting info... [enabled={info.Enabled}, supportedWidth={info.SupportedWidth}, supportedHeight={info.SupportedHeight}, supportedFormat={(int)info.SupportedFormat}]");

        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<BootLogoInfo>());

        try
        {
            if (!TokenManipulator.AddPrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges.");

                throw new CantSetUEFIPrivilegeException();
            }

            Marshal.StructureToPtr(info, ptr, false);
            var ptrSize = (uint)Marshal.SizeOf<BootLogoInfo>();

            if (!PInvoke.SetFirmwareEnvironmentVariableEx(LBLDESP, LBLDESP_GUID, ptr.ToPointer(), ptrSize, SCOPE_ATTR))
                PInvokeExtensions.ThrowIfWin32Error("SetFirmwareEnvironmentVariableEx");

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Info set. [enabled={info.Enabled}, supportedWidth={info.SupportedWidth}, supportedHeight={info.SupportedHeight}, supportedFormat={(int)info.SupportedFormat}]");
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);

            TokenManipulator.RemovePrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE);
        }
    }

    private static unsafe uint GetChecksum()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting checksum...");

        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<BootLogoChecksum>());

        try
        {
            if (!TokenManipulator.AddPrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges.");

                throw new CantSetUEFIPrivilegeException();
            }

            var ptrSize = (uint)Marshal.SizeOf<BootLogoChecksum>();

            var size = PInvoke.GetFirmwareEnvironmentVariableEx(LBLDVC, LBLDVC_GUID, ptr.ToPointer(), ptrSize, null);
            if (size != ptrSize)
                PInvokeExtensions.ThrowIfWin32Error("GetFirmwareEnvironmentVariableEx");

            var checksum = Marshal.PtrToStructure<BootLogoChecksum>(ptr).Crc;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Retrieved checksum. [checksum={checksum:X2}]");

            return checksum;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);

            TokenManipulator.RemovePrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE);
        }
    }

    private static unsafe void SetChecksum(uint checksum)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting checksum... [checksum={checksum:X2}]");

        var str = new BootLogoChecksum { Crc = checksum };
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<BootLogoChecksum>());

        try
        {
            if (!TokenManipulator.AddPrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot set UEFI privileges.");

                throw new CantSetUEFIPrivilegeException();
            }

            Marshal.StructureToPtr(str, ptr, false);
            var ptrSize = (uint)Marshal.SizeOf<BootLogoChecksum>();

            if (!PInvoke.SetFirmwareEnvironmentVariableEx(LBLDVC, LBLDVC_GUID, ptr.ToPointer(), ptrSize, SCOPE_ATTR))
                PInvokeExtensions.ThrowIfWin32Error("SetFirmwareEnvironmentVariableEx");

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Checksum set. [checksum={checksum:X2}]");
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);

            TokenManipulator.RemovePrivileges(TokenManipulator.SE_SYSTEM_ENVIRONMENT_PRIVILEGE);
        }
    }

    private static async Task<uint> CopyMyLogoAsync(BootLogoInfo info, string sourcePath)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Copying logo... [sourcePath={sourcePath}, enabled={info.Enabled}, supportedWidth={info.SupportedWidth}, supportedHeight={info.SupportedHeight}, supportedFormat={(int)info.SupportedFormat}]");

        char? drive = null;

        try
        {
            drive = await MountEfiPartitionAsync().ConfigureAwait(false);
            if (!drive.HasValue)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot mount EFI partition.");

                throw new CantMountUEFIPartitionException();
            }

            if (new DriveInfo($"{drive}:").AvailableFreeSpace < new FileInfo(sourcePath).Length)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Not enough free space on EFI partition.");

                throw new NotEnoughSpaceOnUEFIPartitionException();
            }

            var destinationDirectory = Path.Combine($"{drive}:", "EFI", "Lenovo", "Logo");
            var filename = $"mylogo_{info.SupportedWidth}x{info.SupportedHeight}{Path.GetExtension(sourcePath)}";
            var destinationPath = Path.Combine(destinationDirectory, filename);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Destination path: {destinationPath}");

            Directory.CreateDirectory(destinationDirectory);
            File.Copy(sourcePath, destinationPath, true);

            var checksum = Crc32Adler.Calculate(destinationPath);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Logo copied. [checksum={checksum:X2}]");

            return checksum;
        }
        finally
        {
            if (drive.HasValue)
                await UnMountEfiPartitionAsync(drive.Value).ConfigureAwait(false);
        }
    }

    private static async Task DeleteMyLogoAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Deleting logos...");

        char? drive = null;

        try
        {
            drive = await MountEfiPartitionAsync().ConfigureAwait(false);
            if (!drive.HasValue)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Cannot mount EFI partition");

                throw new CantMountUEFIPartitionException();
            }

            var directoryPath = $@"{drive}:\EFI\Lenovo\Logo";
            if (!Directory.Exists(directoryPath))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"No logos to delete.");

                return;
            }

            var files = Directory.EnumerateFiles(directoryPath, "mylogo*").ToArray();
            files.ForEach(File.Delete);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Logos deleted. [count={files.Length}]");
        }
        finally
        {
            if (drive.HasValue)
                await UnMountEfiPartitionAsync(drive.Value).ConfigureAwait(false);
        }
    }

    private static void ThrowIfImageInvalid(BootLogoInfo info, string sourcePath)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Validating image... [sourcePath={sourcePath}, sourcePath={sourcePath}, enabled={info.Enabled}, supportedWidth={info.SupportedWidth}, supportedHeight={info.SupportedHeight}, supportedFormat={(int)info.SupportedFormat}]");

        using var image = Image.FromFile(sourcePath);

        if (info.SupportedWidth != image.Width || info.SupportedHeight != image.Height)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Invalid image size.");

            throw new InvalidBootLogoImageSizeException();
        }

        if (!info.SupportedFormat.ImageFormats().Contains(image.RawFormat))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Invalid image format.");

            throw new InvalidBootLogoImageFormatException();
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Image valid. [sourcePath={sourcePath}, sourcePath={sourcePath}, enabled={info.Enabled}, supportedWidth={info.SupportedWidth}, supportedHeight={info.SupportedHeight}, supportedFormat={(int)info.SupportedFormat}]");
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
        if (result != 0)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to mount EFI partition at {drive}:");

            return null;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"EFI partition mounted at {drive}:");

        return drive;
    }

    private static async Task UnMountEfiPartitionAsync(char letter)
    {
        var (result, _) = await CMD.RunAsync("mountvol", $"{letter}: /d").ConfigureAwait(false);

        if (result != 0)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to un-mount EFI partition from {letter}:");
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"EFI partition un-mounted from {letter}:.");
    }
}
