namespace LenovoLegionToolkit.Lib
{
    public enum AlwaysOnUsbState
    {
        Off,
        OnWhenSleeping,
        OnAlways
    }

    public enum BatteryState
    {
        Conservation,
        Normal,
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

    public enum Key
    {
        Unknown = 0,
        Fn_F9 = 1,
        Fn_LockOn = 2,
        Fn_LockOff = 3,
        Fn_PrtSc = 4,
        Fn_R = 16
    }
}
