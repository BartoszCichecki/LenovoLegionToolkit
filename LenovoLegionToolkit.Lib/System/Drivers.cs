﻿using System;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.System
{
    internal static class Drivers
    {
        private static readonly object _locker = new();

        private static SafeFileHandle? _energy;

        public static SafeFileHandle GetEnergy()
        {
            if (_energy is null)
            {
                lock (_locker)
                {
                    if (_energy is null)
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
    }
}
