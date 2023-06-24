using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LenovoLegionToolkit.Lib.Utils;

// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.WPF;

public class Flags
{
    public bool IsTraceEnabled { get; }
    public bool Minimized { get; }
    public bool SkipCompatibilityCheck { get; }
    public bool DisableTrayTooltip { get; }
    public bool AllowAllPowerModesOnBattery { get; }
    public bool ForceDisableRgbKeyboardSupport { get; }
    public bool ForceDisableSpectrumKeyboardSupport { get; }
    public bool ForceDisableLenovoLighting { get; }

    public Flags(IEnumerable<string> startupArgs)
    {
        var args = startupArgs.Concat(LoadExternalArgs()).ToArray();

        IsTraceEnabled = args.Contains("--trace");
        Minimized = args.Contains("--minimized");
        SkipCompatibilityCheck = args.Contains("--skip-compat-check");
        DisableTrayTooltip = args.Contains("--disable-tray-tooltip");
        AllowAllPowerModesOnBattery = args.Contains("--allow-all-power-modes-on-battery");
        ForceDisableRgbKeyboardSupport = args.Contains("--force-disable-rgbkb");
        ForceDisableSpectrumKeyboardSupport = args.Contains("--force-disable-spectrumkb");
        ForceDisableLenovoLighting = args.Contains("--force-disable-lenovolighting");
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

    public override string ToString() =>
        $"{nameof(IsTraceEnabled)}: {IsTraceEnabled}," +
        $" {nameof(Minimized)}: {Minimized}," +
        $" {nameof(SkipCompatibilityCheck)}: {SkipCompatibilityCheck}," +
        $" {nameof(DisableTrayTooltip)}: {DisableTrayTooltip}," +
        $" {nameof(AllowAllPowerModesOnBattery)}: {AllowAllPowerModesOnBattery}," +
        $" {nameof(ForceDisableRgbKeyboardSupport)}: {ForceDisableRgbKeyboardSupport}," +
        $" {nameof(ForceDisableSpectrumKeyboardSupport)}: {ForceDisableSpectrumKeyboardSupport}," +
        $" {nameof(ForceDisableLenovoLighting)}: {ForceDisableLenovoLighting}";
}
