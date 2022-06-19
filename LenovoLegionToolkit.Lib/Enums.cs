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
        RapidCharge
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

    public enum HybridModeState
    {
        On,
        Off
    }

    public enum OverDriveState
    {
        Off,
        On
    }

    public enum PowerModeState
    {
        Quiet,
        Balance,
        Performance
    }

    public enum ProcessEventInfoType
    {
        Started,
        Stopped,
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
        Dark
    }

    public enum TemperatureUnit
    {
        C,
        F,
    }

    public enum TouchpadLockState
    {
        Off,
        On
    }

    public enum VantageStatus
    {
        Enabled,
        Disabled,
        NotFound
    }
}
