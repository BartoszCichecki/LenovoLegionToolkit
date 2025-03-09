using System.Diagnostics;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class ProcessExtensions
{
    public static string? GetFileName(this Process process, int maxLength = 512)
    {
        var chars = new char[maxLength];
        return PInvoke.K32GetModuleFileNameEx(process.SafeHandle, null, chars) == 0
            ? null
            : chars.ToString();
    }
}
