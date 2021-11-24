using System;
using Microsoft.Win32.SafeHandles;

namespace LenovoLegionToolkit.Lib.Utils
{
    public static class Drivers
    {
        private static SafeFileHandle _energy;
        public static SafeFileHandle Energy
        {
            get
            {
                if (_energy == null)
                {
                    var fileHandle = Native.CreateFileW("\\\\.\\EnergyDrv", 0xC0000000, 3u, IntPtr.Zero, 3u, 0x80, IntPtr.Zero);
                    if (fileHandle == new IntPtr(-1))
                        throw new InvalidOperationException("fileHandle is 0");
                    _energy = new SafeFileHandle(fileHandle, true);
                }
                return _energy;
            }
        }
    }
}
