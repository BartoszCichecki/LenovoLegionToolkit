namespace LenovoLegionToolkit.Lib
{
    public enum Theme
    {
        System,
        Light,
        Dark
    }

    public enum AlwaysOnUsbState
    {
        Off,
        OnWhenSleeping,
        OnAlways
    }

    public static class AlwaysOnUsbStateExtension
    {
        public static string DisplayName(this AlwaysOnUsbState state) => state switch
        {
            AlwaysOnUsbState.OnWhenSleeping => "On, when sleeping",
            AlwaysOnUsbState.OnAlways => "On, always",
            _ => state.ToString(),
        };
    }


    public enum BatteryState
    {
        Conservation,
        Normal,
        RapidCharge
    }

    public static class BatteryStateExtension
    {
        public static string DisplayName(this BatteryState state) => state switch
        {
            BatteryState.RapidCharge => "Rapid Charge",
            _ => state.ToString(),
        };
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
