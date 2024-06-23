using System;
using System.Linq;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.CLI.Features;

public static class FeatureRegistry
{
    public static readonly IFeatureRegistration[] All =
    [
        new FeatureRegistration<AlwaysOnUSBState>("always-on-usb"),
        new FeatureRegistration<BatteryState>("battery"),
        new FeatureRegistration<BatteryNightChargeState>("battery-night-charge"),
        new FeatureRegistration<FlipToStartState>("flip-to-start"),
        new FeatureRegistration<FnLockState>("fn-lock"),
        new FeatureRegistration<HDRState>("hdr"),
        new FeatureRegistration<HybridModeState>("hybrid-mode"),
        new FeatureRegistration<InstantBootState>("instant-boot"),
        new FeatureRegistration<MicrophoneState>("microphone"),
        new FeatureRegistration<OneLevelWhiteKeyboardBacklightState>("one-level-white-keyboard-backlight"),
        new FeatureRegistration<OverDriveState>("over-drive"),
        new FeatureRegistration<PanelLogoBacklightState>("panel-logo-backlight"),
        new FeatureRegistration<PortsBacklightState>("ports-backlight"),
        new FeatureRegistration<PowerModeState>("power-mode"),
        new FeatureRegistration<RefreshRate>("refresh-rate",
            s => s.Frequency.ToString(),
            s => new(Convert.ToInt32(s))),
        new FeatureRegistration<Resolution>("resolution",
            s => $"{s.Width}x{s.Height}",
            s =>
            {
                var split = s.Split('x');
                return new(Convert.ToInt32(split.FirstOrDefault()), Convert.ToInt32(split.LastOrDefault()));
            }),
        new FeatureRegistration<SpeakerState>("speaker"),
        new FeatureRegistration<TouchpadLockState>("touchpad-lock"),
        new FeatureRegistration<WinKeyState>("win-key"),
        new FeatureRegistration<WhiteKeyboardBacklightState>("white-keyboard-backlight"),
    ];
}
