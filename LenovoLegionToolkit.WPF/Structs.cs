using System;
using System.Windows.Controls;
using LenovoLegionToolkit.WPF.Controls.Dashboard;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF;

public readonly struct DashboardGroup
{
    public static readonly DashboardGroup[] DefaultGroups = {
        new(DashboardGroupType.Power,
            null,
            DashboardItem.PowerMode,
            DashboardItem.BatteryMode,
            DashboardItem.AlwaysOnUsb),
        new(DashboardGroupType.Graphics,
            null,
            DashboardItem.HybridMode,
            DashboardItem.DiscreteGpu),
        new(DashboardGroupType.Display,
            null,
            DashboardItem.Resolution,
            DashboardItem.RefreshRate,
            DashboardItem.DpiScale,
            DashboardItem.Hdr,
            DashboardItem.OverDrive),
        new(DashboardGroupType.Other,
            null,
            DashboardItem.Microphone,
            DashboardItem.FlipToStart,
            DashboardItem.TouchpadLock,
            DashboardItem.FnLock,
            DashboardItem.WinKeyLock)
    };

    public DashboardGroupType Type { get; }

    public string? CustomName { get; }

    public DashboardItem[] Items { get; }

    public DashboardGroup(DashboardGroupType type, string? customName, params DashboardItem[] items)
    {
        Type = type;
        CustomName = type == DashboardGroupType.Custom ? customName : null;
        Items = items;
    }

    public string GetName() => Type switch
    {
        DashboardGroupType.Power => Resource.DashboardPage_Power_Title,
        DashboardGroupType.Graphics => Resource.DashboardPage_Graphics_Title,
        DashboardGroupType.Display => Resource.DashboardPage_Display_Title,
        DashboardGroupType.Other => Resource.DashboardPage_Other_Title,
        DashboardGroupType.Custom => CustomName ?? string.Empty,
        _ => throw new InvalidOperationException($"Invalid type {Type}"),
    };
}

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
    AlwaysOnUsb,
    HybridMode,
    DiscreteGpu,
    Resolution,
    RefreshRate,
    DpiScale,
    Hdr,
    OverDrive,
    Microphone,
    FlipToStart,
    TouchpadLock,
    FnLock,
    WinKeyLock
}

public static class DashboardItemExtensions
{
    public static SymbolRegular GetIcon(this DashboardItem dashboardItem) => dashboardItem switch
    {
        DashboardItem.PowerMode => SymbolRegular.Gauge24,
        DashboardItem.BatteryMode => SymbolRegular.BatteryCharge24,
        DashboardItem.AlwaysOnUsb => SymbolRegular.UsbStick24,
        DashboardItem.HybridMode => SymbolRegular.LeafOne24,
        DashboardItem.DiscreteGpu => SymbolRegular.DeveloperBoard24,
        DashboardItem.Resolution => SymbolRegular.ScaleFill24,
        DashboardItem.RefreshRate => SymbolRegular.DesktopPulse24,
        DashboardItem.DpiScale => SymbolRegular.TextFontSize24,
        DashboardItem.Hdr => SymbolRegular.Hdr24,
        DashboardItem.OverDrive => SymbolRegular.TopSpeed24,
        DashboardItem.Microphone => SymbolRegular.Mic24,
        DashboardItem.FlipToStart => SymbolRegular.Power24,
        DashboardItem.TouchpadLock => SymbolRegular.Tablet24,
        DashboardItem.FnLock => SymbolRegular.Keyboard24,
        DashboardItem.WinKeyLock => SymbolRegular.Keyboard24,
        _ => throw new InvalidOperationException($"Invalid DashboardItem {dashboardItem}"),
    };

    public static string GetTitle(this DashboardItem dashboardItem) => dashboardItem switch
    {
        DashboardItem.PowerMode => Resource.PowerModeControl_Title,
        DashboardItem.BatteryMode => Resource.BatteryModeControl_Title,
        DashboardItem.AlwaysOnUsb => Resource.AlwaysOnUSBControl_Title,
        DashboardItem.HybridMode => $"{Resource.ComboBoxHybridModeControl_Title} / {Resource.ToggleHybridModeControl_Title}",
        DashboardItem.DiscreteGpu => Resource.DiscreteGPUControl_Title,
        DashboardItem.Resolution => Resource.ResolutionControl_Title,
        DashboardItem.RefreshRate => Resource.RefreshRateControl_Title,
        DashboardItem.DpiScale => Resource.DpiScaleControl_Title,
        DashboardItem.Hdr => Resource.HDRControl_Title,
        DashboardItem.OverDrive => Resource.OverDriveControl_Title,
        DashboardItem.Microphone => Resource.MicrophoneControl_Title,
        DashboardItem.FlipToStart => Resource.FlipToStartControl_Title,
        DashboardItem.TouchpadLock => Resource.TouchpadLockControl_Title,
        DashboardItem.FnLock => Resource.FnLockControl_Title,
        DashboardItem.WinKeyLock => Resource.WinKeyControl_Title,
        _ => throw new InvalidOperationException($"Invalid DashboardItem {dashboardItem}"),
    };

    public static ContentControl GetControl(this DashboardItem dashboardItem) => dashboardItem switch
    {
        DashboardItem.PowerMode => new PowerModeControl(),
        DashboardItem.BatteryMode => new BatteryModeControl(),
        DashboardItem.AlwaysOnUsb => new AlwaysOnUSBControl(),
        DashboardItem.HybridMode => new HybridModeControl(),
        DashboardItem.DiscreteGpu => new DiscreteGPUControl(),
        DashboardItem.Resolution => new ResolutionControl(),
        DashboardItem.RefreshRate => new RefreshRateControl(),
        DashboardItem.DpiScale => new DpiScaleControl(),
        DashboardItem.Hdr => new HDRControl(),
        DashboardItem.OverDrive => new OverDriveControl(),
        DashboardItem.Microphone => new MicrophoneControl(),
        DashboardItem.FlipToStart => new FlipToStartControl(),
        DashboardItem.TouchpadLock => new TouchpadLockControl(),
        DashboardItem.FnLock => new FnLockControl(),
        DashboardItem.WinKeyLock => new WinKeyControl(),
        _ => throw new InvalidOperationException($"Invalid DashboardItem {dashboardItem}"),
    };
}
