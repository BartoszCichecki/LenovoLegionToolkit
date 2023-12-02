using System;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;

namespace LenovoLegionToolkit.Lib.System;

public static class Drivers
{
    public const uint IOCTL_ENERGY_BATTERY_INFORMATION = 0x83102138;
    public const uint IOCTL_ENERGY_SETTINGS = 0x831020E8;
    public const uint IOCTL_ENERGY_BATTERY_CHARGE_MODE = 0x831020F8;
    public const uint IOCTL_ENERGY_BATTERY_NIGHT_CHARGE = 0x83102150;
    public const uint IOCTL_ENERGY_KEYBOARD = 0x83102144;
    public const uint IOCTL_KEY_WAIT_HANDLE = 0x831020D8;
    public const uint IOCTL_KEY_VALUE = 0x831020CC;

    private static readonly object Lock = new();

    private static SafeFileHandle? _energy;

    public static SafeFileHandle GetEnergy()
    {
        if (_energy is not null)
            return _energy;

        lock (Lock)
        {
            if (_energy is not null)
                return _energy;

            var handle = PInvoke.CreateFile(@"\\.\EnergyDrv",
                (uint)FILE_ACCESS_RIGHTS.FILE_READ_DATA | (uint)FILE_ACCESS_RIGHTS.FILE_WRITE_DATA,
                FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                null,
                FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                null);

            if (handle.IsInvalid)
                throw new InvalidOperationException("handle is invalid");

            _energy = handle;
        }

        return _energy;
    }
}
