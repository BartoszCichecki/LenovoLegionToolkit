using System.ComponentModel.DataAnnotations;

namespace LenovoLegionToolkit.Lib
{
    public enum AlwaysOnUSBState
    {
        Off,
        [Display(Name = "On, when sleeping")]
        OnWhenSleeping,
        [Display(Name = "On, always")]
        OnAlways
    }


    public enum BatteryState
    {
        Conservation,
        Normal,
        [Display(Name = "Rapid Charge")]
        RapidCharge
    }

    public enum DriverKey
    {
        Fn_F4 = 256,
        Fn_F8 = 8192,
        Fn_F10 = 32
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
        [Display(Name = "Hybrid", Description = "Automatically switch between iGPU and dGPU.")]
        On,
        [Display(Name = "Hybrid-iGPU", Description = "Use iGPU only.")]
        OnIGPUOnly,
        [Display(Name = "Hybrid-Auto", Description = "Use both iGPU and dGPU when on AC power. Use iGPU only on battery.")]
        OnAuto,
        [Display(Name = "dGPU", Description = "Use dGPU only.")]
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

    public enum NotificationDuration
    {
        Short,
        Long
    }

    public enum NotificationIcon
    {
        MicrophoneOff,
        MicrophoneOn,
        RefreshRate,
        TouchpadOn,
        TouchpadOff,
        CameraOn,
        CameraOff
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
        Quiet,
        Balance,
        Performance,
        [Display(Name = "Custom")]
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
        Low,
        High
    }

    public enum RGBKeyboardEffect
    {
        Static,
        Breath,
        Smooth,
        [Display(Name = "Wave Right")]
        WaveRTL,
        [Display(Name = "Wave Left")]
        WaveLTR
    }

    public enum RGBKeyboardBacklightPreset
    {
        Off = -1,
        [Display(Name = "Preset 1")]
        One = 0,
        [Display(Name = "Preset 2")]
        Two = 1,
        [Display(Name = "Preset 3")]
        Three = 2
    }

    public enum RBGKeyboardSpeed
    {
        Slowest,
        Slow,
        Fast,
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
        Unknown = 0,
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
        System,
        Light,
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

    public enum WhiteKeyboardBacklightChanged { }

    public enum WhiteKeyboardBacklightState
    {
        Off,
        Low,
        High
    }

    public enum WinKeyState
    {
        Off,
        On
    }

    public enum WinKeyChanged { }
}
