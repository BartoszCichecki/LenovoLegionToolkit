using System;
using System.ComponentModel.DataAnnotations;
using LenovoLegionToolkit.Lib.Resources;

namespace LenovoLegionToolkit.Lib;

public enum AlwaysOnUSBState
{
    [Display(ResourceType = typeof(Resource), Name = "AlwaysOnUSBState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "AlwaysOnUSBState_OnWhenSleeping")]
    OnWhenSleeping,
    [Display(ResourceType = typeof(Resource), Name = "AlwaysOnUSBState_OnAlways")]
    OnAlways
}

public enum AutorunState
{
    [Display(ResourceType = typeof(Resource), Name = "AutorunState_Enabled")]
    Enabled,
    [Display(ResourceType = typeof(Resource), Name = "AutorunState_EnabledDelayed")]
    EnabledDelayed,
    [Display(ResourceType = typeof(Resource), Name = "AutorunState_Disabled")]
    Disabled
}

public enum BatteryState
{
    [Display(ResourceType = typeof(Resource), Name = "BatteryState_Conservation")]
    Conservation,
    [Display(ResourceType = typeof(Resource), Name = "BatteryState_Normal")]
    Normal,
    [Display(ResourceType = typeof(Resource), Name = "BatteryState_RapidCharge")]
    RapidCharge
}

[Flags]
public enum DriverKey
{
    Fn_F10 = 32,
    Fn_F4 = 256,
    Fn_F8 = 8192,
    Fn_Space = 4096,
}

public enum FanTableType
{
    Unknown,
    CPU,
    GPU,
    CPUSensor
}

public enum FlipToStartState
{
    Off,
    On
}

public enum FnLockState
{
    Off,
    On
}

public enum GSyncState
{
    On,
    Off
}

public enum HDRState
{
    Off,
    On
}

public enum HybridModeState
{
    [Display(ResourceType = typeof(Resource), Name = "HybridModeState_On")]
    On,
    [Display(ResourceType = typeof(Resource), Name = "HybridModeState_OnIGPUOnly")]
    OnIGPUOnly,
    [Display(ResourceType = typeof(Resource), Name = "HybridModeState_OnAuto")]
    OnAuto,
    [Display(ResourceType = typeof(Resource), Name = "HybridModeState_Off")]
    Off
}

public enum IGPUModeState
{
    Default,
    IGPUOnly,
    Auto
}

public enum KeyboardLayout
{
    Ansi,
    Iso
}

public enum KnownFolder
{
    Contacts,
    Downloads,
    Favorites,
    Links,
    SavedGames,
    SavedSearches
}

public enum LightingChangeState
{
    Panel = 0,
    Ports = 1,
}

public enum MicrophoneState
{
    Off,
    On
}

public enum NativeWindowsMessage
{
    LidOpened,
    LidClosed,
    MonitorOn,
    MonitorOff,
    MonitorConnected,
    MonitorDisconnected,
    OnDisplayDeviceArrival
}

public enum NotificationDuration
{
    Short,
    Long
}

public enum NotificationType
{
    ACAdapterConnected,
    ACAdapterConnectedLowWattage,
    ACAdapterDisconnected,
    CameraOn,
    CameraOff,
    CapsLockOn,
    CapsLockOff,
    FnLockOn,
    FnLockOff,
    MicrophoneOff,
    MicrophoneOn,
    NumLockOn,
    NumLockOff,
    PanelLogoLightingOn,
    PanelLogoLightingOff,
    PortLightingOn,
    PortLightingOff,
    PowerModeQuiet,
    PowerModeBalance,
    PowerModePerformance,
    PowerModeGodMode,
    RefreshRate,
    RGBKeyboardBacklightChanged,
    RGBKeyboardBacklightOff,
    SmartKeyDoublePress,
    SmartKeySinglePress,
    SpectrumBacklightChanged,
    SpectrumBacklightOff,
    SpectrumBacklightPresetChanged,
    TouchpadOn,
    TouchpadOff,
    UpdateAvailable,
    WhiteKeyboardBacklightChanged,
    WhiteKeyboardBacklightOff
}

public enum NotificationPosition
{
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_BottomRight")]
    BottomRight,
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_BottomCenter")]
    BottomCenter,
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_BottomLeft")]
    BottomLeft,
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_CenterLeft")]
    CenterLeft,
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_TopLeft")]
    TopLeft,
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_TopCenter")]
    TopCenter,
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_TopRight")]
    TopRight,
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_CenterRight")]
    CenterRight,
    [Display(ResourceType = typeof(Resource), Name = "NotificationPosition_Center")]
    Center
}

public enum OneLevelWhiteKeyboardBacklightState
{
    Off,
    On
}

public enum OS
{
    [Display(Name = "Windows 11")]
    Windows11,
    [Display(Name = "Windows 10")]
    Windows10,
    [Display(Name = "Windows 8")]
    Windows8,
    [Display(Name = "Windows 7")]
    Windows7
}

public enum OverDriveState
{
    Off,
    On
}

public enum PowerAdapterStatus
{
    Connected,
    ConnectedLowWattage,
    Disconnected
}

public enum PowerModeState
{
    [Display(ResourceType = typeof(Resource), Name = "PowerModeState_Quiet")]
    Quiet,
    [Display(ResourceType = typeof(Resource), Name = "PowerModeState_Balance")]
    Balance,
    [Display(ResourceType = typeof(Resource), Name = "PowerModeState_Performance")]
    Performance,
    [Display(ResourceType = typeof(Resource), Name = "PowerModeState_GodMode")]
    GodMode = 254
}

public enum ProcessEventInfoType
{
    Started,
    Stopped
}

public enum RebootType
{
    NotRequired = 0,
    Forced = 1,
    Requested = 3,
    ForcedPowerOff = 4,
    Delayed = 5
}

public enum RGBKeyboardBacklightChanged { }

public enum RGBKeyboardBacklightBrightness
{
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightBrightness_Low")]
    Low,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightBrightness_High")]
    High
}

public enum RGBKeyboardBacklightEffect
{
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightEffect_Static")]
    Static,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightEffect_Breath")]
    Breath,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightEffect_Smooth")]
    Smooth,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightEffect_WaveRTL")]
    WaveRTL,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightEffect_WaveLTR")]
    WaveLTR
}

public enum RGBKeyboardBacklightPreset
{
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightPreset_Off")]
    Off = -1,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightPreset_One")]
    One = 0,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightPreset_Two")]
    Two = 1,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightPreset_Three")]
    Three = 2
}

public enum RBGKeyboardBacklightSpeed
{
    [Display(ResourceType = typeof(Resource), Name = "RBGKeyboardBacklightSpeed_Slowest")]
    Slowest,
    [Display(ResourceType = typeof(Resource), Name = "RBGKeyboardBacklightSpeed_Slow")]
    Slow,
    [Display(ResourceType = typeof(Resource), Name = "RBGKeyboardBacklightSpeed_Fast")]
    Fast,
    [Display(ResourceType = typeof(Resource), Name = "RBGKeyboardBacklightSpeed_Fastest")]
    Fastest
}

public enum SoftwareStatus
{
    Enabled,
    Disabled,
    NotFound
}

public enum SpecialKey
{
    FnF9 = 1,
    FnLockOn = 2,
    FnLockOff = 3,
    FnPrtSc = 4,
    CameraOn = 12,
    CameraOff = 13,
    FnR = 16,
    FnR2 = 0x0041002A,
    PanelLogoLightingOn = 20,
    PanelLogoLightingOff = 21,
    SpectrumBacklightOff = 24,
    SpectrumBacklight1 = 25,
    SpectrumBacklight2 = 26,
    SpectrumBacklight3 = 38,
    SpectrumPreset1 = 32,
    SpectrumPreset2 = 33,
    SpectrumPreset3 = 34,
    SpectrumPreset4 = 35,
    SpectrumPreset5 = 36,
    SpectrumPreset6 = 37
}

public enum SpectrumKeyboardBacklightBrightness
{
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightBrightness_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightBrightness_Low")]
    Low,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightBrightness_Medium")]
    Medium,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightBrightness_High")]
    High
}

public enum SpectrumKeyboardBacklightClockwiseDirection
{
    None,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightDirection_Clockwise")]
    Clockwise,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightDirection_CounterClockwise")]
    CounterClockwise
}

public enum SpectrumKeyboardBacklightDirection
{
    None,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightDirection_BottomToTop")]
    BottomToTop,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightDirection_TopToBottom")]
    TopToBottom,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightDirection_LeftToRight")]
    LeftToRight,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightDirection_RightToLeft")]
    RightToLeft
}

public enum SpectrumKeyboardBacklightEffectType
{
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_Always")]
    Always,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_RainbowScrew")]
    RainbowScrew,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_RainbowWave")]
    RainbowWave,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_ColorChange")]
    ColorChange,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_ColorWave")]
    ColorWave,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_ColorPulse")]
    ColorPulse,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_Smooth")]
    Smooth,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_Rain")]
    Rain,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_Ripple")]
    Ripple,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_Type")]
    Type,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_AudioBounce")]
    AudioBounce,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_AudioRipple")]
    AudioRipple,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightEffectType_AuroraSync")]
    AuroraSync
}

public enum SpectrumKeyboardBacklightSpeed
{
    None,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightSpeed_Speed1")]
    Speed1,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightSpeed_Speed2")]
    Speed2,
    [Display(ResourceType = typeof(Resource), Name = "SpectrumKeyboardBacklightSpeed_Speed3")]
    Speed3
}

public enum SpectrumLayout
{
    KeyboardOnly,
    KeyboardAndFront,
    Full
}

public enum Theme
{
    [Display(ResourceType = typeof(Resource), Name = "Theme_System")]
    System,
    [Display(ResourceType = typeof(Resource), Name = "Theme_Light")]
    Light,
    [Display(ResourceType = typeof(Resource), Name = "Theme_Dark")]
    Dark
}

public enum AccentColorSource
{
    [Display(ResourceType = typeof(Resource), Name = "AccentColorSource_System")]
    System,
    [Display(ResourceType = typeof(Resource), Name = "AccentColorSource_Custom")]
    Custom
}

public enum TemperatureUnit
{
    C,
    F
}

public enum ThermalModeState
{
    Unknown,
    Quiet,
    Balance,
    Performance,
    GodMode = 255
}

public enum TouchpadLockState
{
    Off,
    On
}

public enum WhiteKeyboardBacklightState
{
    [Display(ResourceType = typeof(Resource), Name = "WhiteKeyboardBacklightState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "WhiteKeyboardBacklightState_Low")]
    Low,
    [Display(ResourceType = typeof(Resource), Name = "WhiteKeyboardBacklightState_High")]
    High
}

public enum WinKeyState
{
    Off,
    On
}

public enum WinKeyChanged { }