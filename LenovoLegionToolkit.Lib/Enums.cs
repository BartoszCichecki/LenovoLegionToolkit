using System.ComponentModel.DataAnnotations;

namespace LenovoLegionToolkit.Lib
{
    public enum AlwaysOnUSBState
    {
        Off,
        [Display(Name = "On, when sleeping")]
        OnWhenSleeping,
        [Display(Name = "On, always")]
        OnAlways,
    }


    public enum BatteryState
    {
        Conservation,
        Normal,
        [Display(Name = "Rapid Charge")]
        RapidCharge,
    }

    public enum DriverKey
    {
        Fn_F4 = 256,
        Fn_F8 = 8192,
        Fn_F10 = 32,
    }

    public enum FlipToStartState
    {
        Off,
        On,
    }

    public enum FnLockState
    {
        Off,
        On,
    }

    public enum HybridModeState
    {
        On,
        Off,
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
        Long,
    }

    public enum NotificationIcon
    {
        MicrophoneOff,
        MicrophoneOn,
        RefreshRate,
        TouchpadOn,
        TouchpadOff,
    }

    public enum OverDriveState
    {
        Off,
        On,
    }

    public enum PowerModeState
    {
        Quiet,
        Balance,
        Performance,
    }

    public enum ProcessEventInfoType
    {
        Started,
        Stopped,
    }

    public enum RGBKeyboardBacklightChanged { }

    public enum RGBKeyboardBrightness
    {
        Low,
        High,
    }

    public enum RGBKeyboardEffect
    {
        Static,
        Breath,
        Smooth,
        [Display(Name = "Wave Right")]
        WaveRTL,
        [Display(Name = "Wave Left")]
        WaveLTR,
    }

    public enum RGBKeyboardBacklightPreset
    {
        Off = -1,
        [Display(Name = "Preset 1")]
        One = 0,
        [Display(Name = "Preset 2")]
        Two = 1,
        [Display(Name = "Preset 3")]
        Three = 2,
    }

    public enum RBGKeyboardSpeed
    {
        Slowest,
        Slow,
        Fast,
        Fastest,
    }

    public enum SoftwareStatus
    {
        Enabled,
        Disabled,
        NotFound,
    }

    public enum SpecialKey
    {
        Unknown = 0,
        Fn_F9 = 1,
        Fn_LockOn = 2,
        Fn_LockOff = 3,
        Fn_PrtSc = 4,
        Fn_R = 16
    }

    public enum Theme
    {
        System,
        Light,
        Dark,
    }

    public enum TemperatureUnit
    {
        C,
        F,
    }

    public enum TouchpadLockState
    {
        Off,
        On,
    }

    public enum WhiteKeyboardBacklightChanged { }

    public enum WhiteKeyboardBacklightState
    {
        Off,
        Low,
        High,
    }

    public enum WinKeyState
    {
        On,
        Off,
    }

    public enum WinKeyChanged { }
}
