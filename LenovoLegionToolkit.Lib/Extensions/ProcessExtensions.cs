using System.Diagnostics;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class ProcessExtensions
{
    public static unsafe string? GetFileName(this Process process, int maxLength = 1024)
    {
        var text = stackalloc char[maxLength];
        var str = new PWSTR(text);
        return PInvoke.K32GetModuleFileNameEx(process.SafeHandle, null, str, (uint)maxLength) == 0
            ? null
            : str.ToString();
    }
}