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

public enum BatteryNightChargeState
{
    [Display(ResourceType = typeof(Resource), Name = "BatteryNightChargeState_On")]
    On,
    [Display(ResourceType = typeof(Resource), Name = "BatteryNightChargeState_Off")]
    Off
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

public enum CapabilityID
{
    IGPUMode = 0x00010000,
    FlipToStart = 0x00030000,
    NvidiaGPUDynamicDisplaySwitching = 0x00040000,
    AMDSmartShiftMode = 0x00050001,
    AMDSkinTemperatureTracking = 0x00050002,
    SupportedPowerModes = 0x00070000,
    LegionZoneSupportVersion = 0x00090000,
    GodModeFnQSwitchable = 0x00100000,
    OverDrive = 0x001A0000,
    AIChip = 0x000E0000,
    IGPUModeChangeStatus = 0x000F0000,
    CPUShortTermPowerLimit = 0x0101FF00,
    CPULongTermPowerLimit = 0x0102FF00,
    CPUPeakPowerLimit = 0x0103FF00,
    CPUTemperatureLimit = 0x0104FF00,
    APUsPPTPowerLimit = 0x0105FF00,
    CPUCrossLoadingPowerLimit = 0x0106FF00,
    CPUPL1Tau = 0x0107FF00,
    GPUPowerBoost = 0x0201FF00,
    GPUConfigurableTGP = 0x0202FF00,
    GPUTemperatureLimit = 0x0203FF00,
    GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = 0x0204FF00,
    GPUToCPUDynamicBoost = 0x020BFF00,
    GPUStatus = 0x02070000,
    GPUDidVid = 0x02090000,
    InstantBootAc = 0x03010001,
    InstantBootUsbPowerDelivery = 0x03010002,
    FanFullSpeed = 0x04020000,
    CpuCurrentFanSpeed = 0x04030001,
    GpuCurrentFanSpeed = 0x04030002,
    CpuCurrentTemperature = 0x05040000,
    GpuCurrentTemperature = 0x05050000
}

[Flags]
public enum DriverKey
{
    FnF10 = 32,
    FnF4 = 256,
    FnF8 = 8192,
    FnSpace = 4096,
}

public enum FanTableType
{
    Unknown,
    CPU,
    CPUSensor,
    GPU,
    GPU2
}

public enum FlipToStartState
{
    [Display(ResourceType = typeof(Resource), Name = "FlipToStartState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "FlipToStartState_On")]
    On
}

public enum FnLockState
{
    [Display(ResourceType = typeof(Resource), Name = "FnLockState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "FnLockState_On")]
    On
}

public enum GPUState
{
    Unknown,
    NvidiaGpuNotFound,
    MonitorConnected,
    Active,
    Inactive,
    PoweredOff
}

public enum GSyncState
{
    Off,
    On
}

public enum HDRState
{
    [Display(ResourceType = typeof(Resource), Name = "HDRState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "HDRState_On")]
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

public enum InstantBootState
{
    [Display(ResourceType = typeof(Resource), Name = "InstantBootState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "InstantBootState_AcAdapter")]
    AcAdapter,
    [Display(ResourceType = typeof(Resource), Name = "InstantBootState_UsbPowerDelivery")]
    UsbPowerDelivery,
    [Display(ResourceType = typeof(Resource), Name = "InstantBootState_AcAdapterAndUsbPowerDelivery")]
    AcAdapterAndUsbPowerDelivery
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
    [Display(ResourceType = typeof(Resource), Name = "MicrophoneState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "MicrophoneState_On")]
    On
}

[Flags]
public enum ModifierKey
{
    None = 0,
    [Display(ResourceType = typeof(Resource), Name = "ModifierKey_Shift")]
    Shift = 1,
    [Display(ResourceType = typeof(Resource), Name = "ModifierKey_Ctrl")]
    Ctrl = 2,
    [Display(ResourceType = typeof(Resource), Name = "ModifierKey_Alt")]
    Alt = 4
}

public enum NativeWindowsMessage
{
    LidOpened,
    LidClosed,
    MonitorOn,
    MonitorOff,
    DeviceConnected,
    DeviceDisconnected,
    MonitorConnected,
    MonitorDisconnected,
    ExternalMonitorConnected,
    ExternalMonitorDisconnected,
    OnDisplayDeviceArrival,
    BatterySaverEnabled
}

public enum NotificationDuration
{
    [Display(ResourceType = typeof(Resource), Name = "NotificationDuration_Short")]
    Short,
    [Display(ResourceType = typeof(Resource), Name = "NotificationDuration_Normal")]
    Normal,
    [Display(ResourceType = typeof(Resource), Name = "NotificationDuration_Long")]
    Long
}

public enum NotificationType
{
    ACAdapterConnected,
    ACAdapterConnectedLowWattage,
    ACAdapterDisconnected,
    AutomationNotification,
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
    [Display(ResourceType = typeof(Resource), Name = "OneLevelWhiteKeyboardBacklightState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "OneLevelWhiteKeyboardBacklightState_On")]
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
    [Display(ResourceType = typeof(Resource), Name = "OverdriveState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "OverdriveState_On")]
    On
}

public enum PanelLogoBacklightState
{
    [Display(ResourceType = typeof(Resource), Name = "PanelLogoBacklightState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "PanelLogoBacklightState_On")]
    On
}

public enum PortsBacklightState
{
    [Display(ResourceType = typeof(Resource), Name = "PortsBacklightState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "PortsBacklightState_On")]
    On
}

public enum PowerAdapterStatus
{
    Connected,
    ConnectedLowWattage,
    Disconnected
}

public enum PowerModeMappingMode
{
    [Display(ResourceType = typeof(Resource), Name = "PowerModeMappingMode_Disabled")]
    Disabled,
    [Display(ResourceType = typeof(Resource), Name = "PowerModeMappingMode_WindowsPowerMode")]
    WindowsPowerMode,
    [Display(ResourceType = typeof(Resource), Name = "PowerModeMappingMode_WindowsPowerPlan")]
    WindowsPowerPlan,
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

public enum PowerStateEvent
{
    Unknown = -1,
    StatusChange,
    Suspend,
    Resume,
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

public enum RGBKeyboardBacklightChanged;

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
    Three = 2,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightPreset_Four")]
    Four = 3
}

public enum RGBKeyboardBacklightSpeed
{
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightSpeed_Slowest")]
    Slowest,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightSpeed_Slow")]
    Slow,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightSpeed_Fast")]
    Fast,
    [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBacklightSpeed_Fastest")]
    Fastest
}

public enum SpeakerState
{
    [Display(ResourceType = typeof(Resource), Name = "SpeakerState_Mute")]
    Mute,
    [Display(ResourceType = typeof(Resource), Name = "SpeakerState_Unmute")]
    Unmute
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
    FnPrtSc2 = 45,
    CameraOn = 12,
    CameraOff = 13,
    FnR = 16,
    FnR2 = 0x0041002A,
    SpectrumBacklightOff = 24,
    SpectrumBacklight1 = 25,
    SpectrumBacklight2 = 26,
    SpectrumBacklight3 = 38,
    SpectrumPreset1 = 32,
    SpectrumPreset2 = 33,
    SpectrumPreset3 = 34,
    SpectrumPreset4 = 35,
    SpectrumPreset5 = 36,
    SpectrumPreset6 = 37,
    FnN = 42,
    FnF4 = 62,
    FnF8 = 63,
    WhiteBacklightOff = 64,
    WhiteBacklight1 = 65,
    WhiteBacklight2 = 66
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
    Full,
    FullAlternative
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
    [Display(ResourceType = typeof(Resource), Name = "TouchpadLockState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "TouchpadLockState_On")]
    On
}

public enum UpdateCheckFrequency
{
    [Display(ResourceType = typeof(Resource), Name = "UpdateCheckFrequency_PerHour")]
    PerHour,
    [Display(ResourceType = typeof(Resource), Name = "UpdateCheckFrequency_PerThreeHours")]
    PerThreeHours,
    [Display(ResourceType = typeof(Resource), Name = "UpdateCheckFrequency_PerTwelveHours")]
    PerTwelveHours,
    [Display(ResourceType = typeof(Resource), Name = "UpdateCheckFrequency_PerDay")]
    PerDay,
    [Display(ResourceType = typeof(Resource), Name = "UpdateCheckFrequency_PerWeek")]
    PerWeek,
    [Display(ResourceType = typeof(Resource), Name = "UpdateCheckFrequency_PerMonth")]
    PerMonth
}

public enum UpdateCheckStatus
{
    Success,
    RateLimitReached,
    Error
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

public enum WindowsPowerMode
{
    [Display(Name = "Best power efficiency")]
    BestPowerEfficiency,
    [Display(Name = "Balanced")]
    Balanced,
    [Display(Name = "Best performance")]
    BestPerformance
}

public enum WinKeyState
{
    [Display(ResourceType = typeof(Resource), Name = "WinKeyState_Off")]
    Off,
    [Display(ResourceType = typeof(Resource), Name = "WinKeyState_On")]
    On
}

public enum WinKeyChanged;
