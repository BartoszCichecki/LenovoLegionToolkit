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
        DashboardItem.BatteryNightChargeMode => SymbolRegular.WeatherMoon24,
        DashboardItem.AlwaysOnUsb => SymbolRegular.UsbStick24,
        DashboardItem.InstantBoot => SymbolRegular.PlugDisconnected24,
        DashboardItem.HybridMode => SymbolRegular.LeafOne24,
        DashboardItem.DiscreteGpu => SymbolRegular.DeveloperBoard24,
        DashboardItem.OverclockDiscreteGpu => SymbolRegular.DeveloperBoardLightning20,
        DashboardItem.Resolution => SymbolRegular.ScaleFill24,
        DashboardItem.RefreshRate => SymbolRegular.DesktopPulse24,
        DashboardItem.DpiScale => SymbolRegular.TextFontSize24,
        DashboardItem.Hdr => SymbolRegular.Hdr24,
        DashboardItem.OverDrive => SymbolRegular.TopSpeed24,
        DashboardItem.PanelLogoBacklight => SymbolRegular.LightbulbCircle24,
        DashboardItem.PortsBacklight => SymbolRegular.UsbPlug24,
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
        DashboardItem.BatteryNightChargeMode => Resource.BatteryNightChargeModeControl_Title,
        DashboardItem.AlwaysOnUsb => Resource.AlwaysOnUSBControl_Title,
        DashboardItem.InstantBoot => Resource.InstantBootControl_Title,
        DashboardItem.HybridMode => $"{Resource.ComboBoxHybridModeControl_Title} / {Resource.ToggleHybridModeControl_Title}",
        DashboardItem.DiscreteGpu => Resource.DiscreteGPUControl_Title,
        DashboardItem.OverclockDiscreteGpu => Resource.OverclockDiscreteGPUControl_Title,
        DashboardItem.Resolution => Resource.ResolutionControl_Title,
        DashboardItem.RefreshRate => Resource.RefreshRateControl_Title,
        DashboardItem.DpiScale => Resource.DpiScaleControl_Title,
        DashboardItem.Hdr => Resource.HDRControl_Title,
        DashboardItem.OverDrive => Resource.OverDriveControl_Title,
        DashboardItem.PanelLogoBacklight => Resource.PanelLogoBacklightControl_Title,
        DashboardItem.PortsBacklight => Resource.PortsBacklightControl_Title,
        DashboardItem.TurnOffMonitors => Resource.TurnOffMonitorsControl_Title,
        DashboardItem.Microphone => Resource.MicrophoneControl_Title,
        DashboardItem.FlipToStart => Resource.FlipToStartControl_Title,
        DashboardItem.TouchpadLock => Resource.TouchpadLockControl_Title,
        DashboardItem.FnLock => Resource.FnLockControl_Title,
        DashboardItem.WinKeyLock => Resource.WinKeyControl_Title,
        DashboardItem.WhiteKeyboardBacklight => Resource.WhiteKeyboardBacklightControl_Title,
        _ => throw new InvalidOperationException($"Invalid DashboardItem {dashboardItem}"),
    };

    public static async Task<IEnumerable<AbstractRefreshingControl>> GetControlAsync(this DashboardItem dashboardItem) => dashboardItem switch
    {
        DashboardItem.PowerMode => [new PowerModeControl()],
        DashboardItem.BatteryMode => [new BatteryModeControl()],
        DashboardItem.BatteryNightChargeMode => [new BatteryNightChargeModeControl()],
        DashboardItem.AlwaysOnUsb => [new AlwaysOnUSBControl()],
        DashboardItem.InstantBoot => [new InstantBootControl()],
        DashboardItem.HybridMode => [await HybridModeControlFactory.GetControlAsync()],
        DashboardItem.DiscreteGpu => [new DiscreteGPUControl()],
        DashboardItem.OverclockDiscreteGpu => [new OverclockDiscreteGPUControl()],
        DashboardItem.Resolution => [new ResolutionControl()],
        DashboardItem.RefreshRate => [new RefreshRateControl()],
        DashboardItem.DpiScale => [new DpiScaleControl()],
        DashboardItem.Hdr => [new HDRControl()],
        DashboardItem.OverDrive => [new OverDriveControl()],
        DashboardItem.PanelLogoBacklight => [new PanelLogoBacklightControl()],
        DashboardItem.PortsBacklight => [new PortsBacklightControl()],
        DashboardItem.TurnOffMonitors => [new TurnOffMonitorsControl()],
        DashboardItem.Microphone => [new MicrophoneControl()],
        DashboardItem.FlipToStart => [new FlipToStartControl()],
        DashboardItem.TouchpadLock => [new TouchpadLockControl()],
        DashboardItem.FnLock => [new FnLockControl()],
        DashboardItem.WinKeyLock => [new WinKeyControl()],
        DashboardItem.WhiteKeyboardBacklight => [new WhiteKeyboardBacklightControl(), new OneLevelWhiteKeyboardBacklightControl()],
        _ => throw new InvalidOperationException($"Invalid DashboardItem {dashboardItem}"),
    };
}
