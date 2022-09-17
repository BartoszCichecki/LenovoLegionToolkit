using System;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class StructExtensions
    {
        public static byte[] ToBytes<T>(this T value) where T : struct
        {
            var size = Marshal.SizeOf(value);
            var arr = new byte[size];
            var ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(value, ptr, false);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }
    }
}
