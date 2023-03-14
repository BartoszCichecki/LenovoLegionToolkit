using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF;

public class Flags
{
    public bool IsTraceEnabled { get; }
    public bool Minimized { get; }
    public bool SkipCompatibilityCheck { get; }
    public bool AllowAllPowerModesOnBattery { get; }
    public bool ForceDisableRgbKeyboardSupport { get; }
    public bool ForceDisableSpectrumKeyboardSupport { get; }

    public Flags(IEnumerable<string> startupArgs)
    {
        var args = startupArgs.Concat(LoadExternalArgs()).ToArray();

        IsTraceEnabled = args.Contains("--trace");
        Minimized = args.Contains("--minimized");
        SkipCompatibilityCheck = args.Contains("--skip-compat-check");
        AllowAllPowerModesOnBattery = args.Contains("--allow-all-power-modes-on-battery");
        ForceDisableRgbKeyboardSupport = args.Contains("--force-disable-rgbkb");
        ForceDisableSpectrumKeyboardSupport = args.Contains("--force-disable-spectrumkb");
    }

    private static IEnumerable<string> LoadExternalArgs()
    {
        try
        {
            var argsFile = Path.Combine(Folders.AppData, "args.txt");
            return !File.Exists(argsFile) ? Array.Empty<string>() : File.ReadAllLines(argsFile);
        }
        catch
        {
            return Array.Empty<string>();
        }
    }
}
