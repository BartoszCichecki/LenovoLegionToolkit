using System;
using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.Utils;

public sealed class StructSafeHandle<T> : SafeHandle where T : struct
{
    private readonly IntPtr _ptr;

    public StructSafeHandle(T str) : base(IntPtr.Zero, true)
    {
        var size = Marshal.SizeOf(typeof(T));
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(str, ptr, false);
        SetHandle(ptr);
        _ptr = ptr;
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        Marshal.FreeHGlobal(_ptr);
        return true;
    }
}
