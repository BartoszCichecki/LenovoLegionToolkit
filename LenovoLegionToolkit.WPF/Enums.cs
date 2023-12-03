namespace LenovoLegionToolkit.WPF;

public enum DashboardGroupType
{
    Power,
    Graphics,
    Display,
    Other,
    Custom
}

public enum DashboardItem
{
    PowerMode,
    BatteryMode,
    BatteryNightChargeMode,
    AlwaysOnUsb,
    InstantBoot,
    HybridMode,
    DiscreteGpu,
    OverclockDiscreteGpu,
    PanelLogoBacklight,
    PortsBacklight,
    Resolution,
    RefreshRate,
    DpiScale,
    Hdr,
    OverDrive,
    TurnOffMonitors,
    Microphone,
    FlipToStart,
    TouchpadLock,
    FnLock,
    WinKeyLock,
    WhiteKeyboardBacklight
}

public enum SnackbarType
{
    Success,
    Warning,
    Error,
    Info
}
