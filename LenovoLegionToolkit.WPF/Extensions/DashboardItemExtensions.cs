using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LenovoLegionToolkit.WPF.Controls;
using LenovoLegionToolkit.WPF.Controls.Dashboard;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Extensions;

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
        DashboardItem.TurnOffMonitors => SymbolRegular.Desktop24,
        DashboardItem.Microphone => SymbolRegular.Mic24,
        DashboardItem.FlipToStart => SymbolRegular.Power24,
        DashboardItem.TouchpadLock => SymbolRegular.Tablet24,
        DashboardItem.FnLock => SymbolRegular.Keyboard24,
        DashboardItem.WinKeyLock => SymbolRegular.Keyboard24,
        DashboardItem.WhiteKeyboardBacklight => SymbolRegular.Keyboard24,
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
        DashboardItem.TurnOffMonitors => Resource.TurnOffMonitorsControl_Title,
        DashboardItem.Microphone => Resource.MicrophoneControl_Title,
        DashboardItem.FlipToStart => Resource.FlipToStartControl_Title,
        DashboardItem.TouchpadLock => Resource.TouchpadLockControl_Title,
        DashboardItem.FnLock => Resource.FnLockControl_Title,
        DashboardItem.WinKeyLock => Resource.WinKeyControl_Title,
        DashboardItem.WhiteKeyboardBacklight => $"{Resource.WhiteKeyboardBacklightControl_Title} / {Resource.OneLevelWhiteKeyboardBacklightControl_Title}",
        _ => throw new InvalidOperationException($"Invalid DashboardItem {dashboardItem}"),
    };

    public static async Task<IEnumerable<AbstractRefreshingControl>> GetControlAsync(this DashboardItem dashboardItem) => dashboardItem switch
    {
        DashboardItem.PowerMode => new[] { new PowerModeControl() },
        DashboardItem.BatteryMode => new[] { new BatteryModeControl() },
        DashboardItem.AlwaysOnUsb => new[] { new AlwaysOnUSBControl() },
        DashboardItem.HybridMode => new[] { await HybridModeControlFactory.GetControlAsync() },
        DashboardItem.DiscreteGpu => new[] { new DiscreteGPUControl() },
        DashboardItem.Resolution => new[] { new ResolutionControl() },
        DashboardItem.RefreshRate => new[] { new RefreshRateControl() },
        DashboardItem.DpiScale => new[] { new DpiScaleControl() },
        DashboardItem.Hdr => new[] { new HDRControl() },
        DashboardItem.OverDrive => new[] { new OverDriveControl() },
        DashboardItem.TurnOffMonitors => new[] { new TurnOffMonitorsControl() },
        DashboardItem.Microphone => new[] { new MicrophoneControl() },
        DashboardItem.FlipToStart => new[] { new FlipToStartControl() },
        DashboardItem.TouchpadLock => new[] { new TouchpadLockControl() },
        DashboardItem.FnLock => new[] { new FnLockControl() },
        DashboardItem.WinKeyLock => new[] { new WinKeyControl() },
        DashboardItem.WhiteKeyboardBacklight => new AbstractRefreshingControl[] { new WhiteKeyboardBacklightControl(), new OneLevelWhiteKeyboardBacklightControl() },
        _ => throw new InvalidOperationException($"Invalid DashboardItem {dashboardItem}"),
    };
}
