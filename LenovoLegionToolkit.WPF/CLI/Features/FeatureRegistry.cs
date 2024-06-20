using System.Collections.Generic;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.CLI.Features;

public static class FeatureRegistry
{
    public static readonly List<IFeatureRegistration> All =
    [
        new FeatureRegistration<AlwaysOnUSBState>(),
        new FeatureRegistration<BatteryState>(),
        new FeatureRegistration<BatteryNightChargeState>(),
        new FeatureRegistration<FlipToStartState>(),
        new FeatureRegistration<FnLockState>(),
        new FeatureRegistration<HDRState>(),
        new FeatureRegistration<HybridModeState>(),
        new FeatureRegistration<InstantBootState>(),
        new FeatureRegistration<MicrophoneState>(),
        new FeatureRegistration<OneLevelWhiteKeyboardBacklightState>(),
        new FeatureRegistration<OverDriveState>(),
        new FeatureRegistration<PanelLogoBacklightState>(),
        new FeatureRegistration<PortsBacklightState>(),
        new FeatureRegistration<PowerModeState>(),
        new FeatureRegistration<SpeakerState>(),
        new FeatureRegistration<TouchpadLockState>(),
        new FeatureRegistration<WinKeyState>(),
        new FeatureRegistration<WhiteKeyboardBacklightState>(),
    ];
}