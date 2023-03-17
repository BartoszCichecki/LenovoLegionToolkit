using Autofac;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.PackageDownloader;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib;

public class IoCModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<FnKeys>();
        builder.Register<LegionZone>();
        builder.Register<Vantage>();

        builder.Register<ApplicationSettings>();
        builder.Register<BalanceModeSettings>();
        builder.Register<GodModeSettings>();
        builder.Register<PackageDownloaderSettings>();
        builder.Register<RGBKeyboardSettings>();
        builder.Register<SpectrumKeyboardSettings>();
        builder.Register<SunriseSunsetSettings>();

        builder.Register<AlwaysOnUSBFeature>();
        builder.Register<BatteryFeature>();
        builder.Register<DpiScaleFeature>();
        builder.Register<FlipToStartFeature>();
        builder.Register<FnLockFeature>();
        builder.Register<GSyncFeature>();
        builder.Register<HDRFeature>();
        builder.Register<HybridModeFeature>();
        builder.Register<IGPUModeFeature>();
        builder.Register<MicrophoneFeature>();
        builder.Register<OneLevelWhiteKeyboardBacklightFeature>();
        builder.Register<OverDriveFeature>();
        builder.Register<PowerModeFeature>();
        builder.Register<RefreshRateFeature>();
        builder.Register<ResolutionFeature>();
        builder.Register<TouchpadLockFeature>();
        builder.Register<WhiteKeyboardBacklightFeature>();
        builder.Register<WinKeyFeature>();

        builder.Register<DisplayBrightnessListener>().AutoActivateListener();
        builder.Register<DisplayConfigurationListener>().AutoActivateListener();
        builder.Register<DriverKeyListener>().AutoActivateListener();
        builder.Register<LightingChangeListener>().AutoActivateListener();
        builder.Register<NativeWindowsMessageListener>().AutoActivateListener();
        builder.Register<PowerModeListener>().AutoActivateListener();
        builder.Register<PowerPlanListener>().AutoActivateListener();
        builder.Register<PowerStateListener>().AutoActivateListener();
        builder.Register<RGBKeyboardBacklightListener>().AutoActivateListener();
        builder.Register<SpecialKeyListener>().AutoActivateListener();
        builder.Register<SystemThemeListener>().AutoActivateListener();
        builder.Register<ThermalModeListener>().AutoActivateListener();
        builder.Register<WinKeyListener>().AutoActivateListener();

        builder.Register<AIModeController>();
        builder.Register<CPUBoostModeController>();
        builder.Register<DisplayBrightnessController>();
        builder.Register<GodModeController>();
        builder.Register<GodModeControllerV1>();
        builder.Register<GodModeControllerV2>();
        builder.Register<GPUController>();
        builder.Register<PowerPlanController>();
        builder.Register<RGBKeyboardBacklightController>();
        builder.Register<SpectrumKeyboardBacklightController>();

        builder.Register<UpdateChecker>();
        builder.Register<WarrantyChecker>();

        builder.Register<PCSupportPackageDownloader>();
        builder.Register<VantagePackageDownloader>();
        builder.Register<PackageDownloaderFactory>();

        builder.Register<SunriseSunset>();
    }
}
