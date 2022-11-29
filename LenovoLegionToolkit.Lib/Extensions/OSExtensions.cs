using System;

namespace LenovoLegionToolkit.Lib.Extensions;

public class OSExtensions
{
    public static OS GetCurrent()
    {
        var os = Environment.OSVersion;
        if (os.Version >= new Version(10, 0, 22000, 0)) // Windows 11
            return OS.Windows11;
        else if (os.Version >= new Version(10, 0, 0, 0)) // Windows 10
            return OS.Windows10;
        else if (os.Version >= new Version(6, 2, 0, 0)) // Windows 8
            return OS.Windows8;
        else if (os.Version >= new Version(6, 1, 0, 0)) // Windows 7
            return OS.Windows7;
        else
            return OS.Windows11;
    }
}