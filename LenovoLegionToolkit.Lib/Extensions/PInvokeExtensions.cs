using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class PInvokeExtensions
{
    public const int ERROR_NO_MORE_ITEMS = 259;

    public const uint KF_FLAG_DEFAULT = 0;

    public const uint VK_CAPITAL = 0x14;
    public const uint VK_NUMLOCK = 0x90;

    public const uint VARIABLE_ATTRIBUTE_BOOTSERVICE_ACCESS = 2;
    public const uint VARIABLE_ATTRIBUTE_NON_VOLATILE = 1;
    public const uint VARIABLE_ATTRIBUTE_RUNTIME_ACCESS = 7;

    public static unsafe bool DeviceIoControl<TIn, TOut>(SafeFileHandle hDevice, uint dwIoControlCode, TIn inVal, out TOut outVal) where TIn : struct where TOut : struct
    {
        var lpInBuffer = IntPtr.Zero;
        var lpOutBuffer = IntPtr.Zero;

        try
        {
            var nInBufferSize = Marshal.SizeOf<TIn>();
            var nOutBufferSize = Marshal.SizeOf<TOut>();

            lpInBuffer = Marshal.AllocHGlobal(nInBufferSize);
            lpOutBuffer = Marshal.AllocHGlobal(nOutBufferSize);

            Marshal.StructureToPtr(inVal, lpInBuffer, false);

            var ret = PInvoke.DeviceIoControl(hDevice,
                dwIoControlCode,
                lpInBuffer.ToPointer(),
                (uint)nInBufferSize,
                lpOutBuffer.ToPointer(),
                (uint)nOutBufferSize,
                null,
                null);

            outVal = ret ? Marshal.PtrToStructure<TOut>(lpOutBuffer) : default;

            return ret;
        }
        finally
        {
            Marshal.FreeHGlobal(lpInBuffer);
            Marshal.FreeHGlobal(lpOutBuffer);
        }
    }

    public static void ThrowIfWin32Error(string description)
    {
        var errorCode = Marshal.GetLastWin32Error();
        if (errorCode != 0)
            throw Marshal.GetExceptionForHR(errorCode) ?? throw new Exception($"Unknown Win32 error code {errorCode} in {description}.");

        throw new Exception($"{description} failed but Win32 didn't catch an error.");
    }
}