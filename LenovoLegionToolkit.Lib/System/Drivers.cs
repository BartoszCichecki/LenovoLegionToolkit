using System;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;

namespace LenovoLegionToolkit.Lib.System
{
    internal static class Drivers
    {
        public const uint IOCTL_ENERGY = 0x83102138;

        private static readonly object _locker = new();

        private static SafeFileHandle? _energy;

        public static SafeFileHandle GetEnergy()
        {
            if (_energy is not null)
                return _energy;

            lock (_locker)
            {
                if (_energy is not null)
                    return _energy;

                var handle = PInvoke.CreateFile(@"\\.\EnergyDrv",
                    FILE_ACCESS_FLAGS.FILE_READ_DATA | FILE_ACCESS_FLAGS.FILE_WRITE_DATA,
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
}
