using System.ComponentModel.DataAnnotations;
using LenovoLegionToolkit.Lib.Resources;

namespace LenovoLegionToolkit.Lib
{
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

    public enum DriverKey
    {
        Fn_F4 = 256,
        Fn_F8 = 8192,
        Fn_F8_2 = 8704,
        Fn_F10 = 32,
        Fn_F10_2 = 1056
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
        KeyboardBrightness = 2,
        KeyboardBacklight = 3
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
        PowerMode,
        RefreshRate,
        RGBKeyboardPreset,
        RGBKeyboardPresetOff,
        TouchpadOn,
        TouchpadOff,
        WhiteKeyboardBacklight
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

    public enum RGBKeyboardBacklightChanged { }

    public enum RGBKeyboardBrightness
    {
        [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBrightness_Low")]
        Low,
        [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardBrightness_High")]
        High
    }

    public enum RGBKeyboardEffect
    {
        [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardEffect_Static")]
        Static,
        [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardEffect_Breath")]
        Breath,
        [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardEffect_Smooth")]
        Smooth,
        [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardEffect_WaveRTL")]
        WaveRTL,
        [Display(ResourceType = typeof(Resource), Name = "RGBKeyboardEffect_WaveLTR")]
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

    public enum RBGKeyboardSpeed
    {
        [Display(ResourceType = typeof(Resource), Name = "RBGKeyboardSpeed_Slowest")]
        Slowest,
        [Display(ResourceType = typeof(Resource), Name = "RBGKeyboardSpeed_Slow")]
        Slow,
        [Display(ResourceType = typeof(Resource), Name = "RBGKeyboardSpeed_Fast")]
        Fast,
        [Display(ResourceType = typeof(Resource), Name = "RBGKeyboardSpeed_Fastest")]
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
        Fn_F9 = 1,
        Fn_LockOn = 2,
        Fn_LockOff = 3,
        Fn_PrtSc = 4,
        CameraOn = 12,
        CameraOff = 13,
        Fn_R = 16,
        Fn_R_2 = 0x0041002A
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

    public enum TemperatureUnit
    {
        C,
        F
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
}
