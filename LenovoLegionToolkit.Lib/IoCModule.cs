using Autofac;
using LenovoLegionToolkit.Lib.AutoListeners;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using LenovoLegionToolkit.Lib.Controllers.Sensors;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Features.FlipToStart;
using LenovoLegionToolkit.Lib.Features.Hybrid;
using LenovoLegionToolkit.Lib.Features.Hybrid.Notify;
using LenovoLegionToolkit.Lib.Features.InstantBoot;
using LenovoLegionToolkit.Lib.Features.OverDrive;
using LenovoLegionToolkit.Lib.Features.PanelLogo;
using LenovoLegionToolkit.Lib.Features.WhiteKeyboardBacklight;
using LenovoLegionToolkit.Lib.Integrations;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.PackageDownloader;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib;

public class IoCModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<HttpClientFactory>();

        builder.Register<FnKeysDisabler>();
        builder.Register<LegionZoneDisabler>();
        builder.Register<VantageDisabler>();

        builder.Register<ApplicationSettings>();
        builder.Register<BalanceModeSettings>();
        builder.Register<GodModeSettings>();
        builder.Register<GPUOverclockSettings>();
        builder.Register<IntegrationsSettings>();
        builder.Register<PackageDownloaderSettings>();
        builder.Register<RGBKeyboardSettings>();
        builder.Register<SpectrumKeyboardSettings>();
        builder.Register<SunriseSunsetSettings>();
        builder.Register<UpdateCheckSettings>();

        builder.Register<AlwaysOnUSBFeature>();
        builder.Register<BatteryFeature>();
        builder.Register<BatteryNightChargeFeature>();
        builder.Register<DpiScaleFeature>();
        builder.Register<FlipToStartFeature>();
        builder.Register<FlipToStartCapabilityFeature>(true);
        builder.Register<FlipToStartUEFIFeature>(true);
        builder.Register<FnLockFeature>();
        builder.Register<GSyncFeature>();
        builder.Register<HDRFeature>();
        builder.Register<HybridModeFeature>();
        builder.Register<IGPUModeFeature>();
        builder.Register<IGPUModeCapabilityFeature>(true);
        builder.Register<IGPUModeFeatureFlagsFeature>(true);
        builder.Register<IGPUModeGamezoneFeature>(true);
        builder.Register<InstantBootFeature>();
        builder.Register<InstantBootFeatureFlagsFeature>(true);
        builder.Register<InstantBootCapabilityFeature>(true);
        builder.Register<MicrophoneFeature>();
        builder.Register<OneLevelWhiteKeyboardBacklightFeature>();
        builder.Register<OverDriveFeature>();
        builder.Register<OverDriveGameZoneFeature>(true);
        builder.Register<OverDriveCapabilityFeature>(true);
        builder.Register<PanelLogoBacklightFeature>();
        builder.Register<PanelLogoSpectrumBacklightFeature>(true);
        builder.Register<PanelLogoLenovoLightingBacklightFeature>(true);
        builder.Register<PortsBacklightFeature>();
        builder.Register<PowerModeFeature>();
        builder.Register<RefreshRateFeature>();
        builder.Register<ResolutionFeature>();
        builder.Register<SpeakerFeature>();
        builder.Register<TouchpadLockFeature>();
        builder.Register<WhiteKeyboardBacklightFeature>();
        builder.Register<WhiteKeyboardDriverBacklightFeature>(true);
        builder.Register<WhiteKeyboardLenovoLightingBacklightFeature>(true);
        builder.Register<WinKeyFeature>();

        builder.Register<DGPUNotify>();
        builder.Register<DGPUCapabilityNotify>(true);
        builder.Register<DGPUFeatureFlagsNotify>(true);
        builder.Register<DGPUGamezoneNotify>(true);

        builder.Register<DisplayBrightnessListener>().AutoActivateListener();
        builder.Register<DisplayConfigurationListener>().AutoActivateListener();
        builder.Register<DriverKeyListener>().AutoActivateListener();
        builder.Register<LightingChangeListener>().AutoActivateListener();
        builder.Register<NativeWindowsMessageListener>().AutoActivateListener();
        builder.Register<PowerModeListener>().AutoActivateListener();
        builder.Register<PowerStateListener>().AutoActivateListener();
        builder.Register<RGBKeyboardBacklightListener>().AutoActivateListener();
        builder.Register<SpecialKeyListener>().AutoActivateListener();
        builder.Register<SystemThemeListener>().AutoActivateListener();
        builder.Register<ThermalModeListener>().AutoActivateListener();
        builder.Register<WinKeyListener>().AutoActivateListener();

        builder.Register<GameAutoListener>();
        builder.Register<InstanceStartedEventAutoAutoListener>();
        builder.Register<InstanceStoppedEventAutoAutoListener>();
        builder.Register<ProcessAutoListener>();
        builder.Register<TimeAutoListener>();
        builder.Register<UserInactivityAutoListener>();
        builder.Register<WiFiAutoListener>();

        builder.Register<AIController>();
        builder.Register<DisplayBrightnessController>();
        builder.Register<GodModeController>();
        builder.Register<GodModeControllerV1>(true);
        builder.Register<GodModeControllerV2>(true);
        builder.Register<GPUController>();
        builder.Register<GPUOverclockController>();
        builder.Register<RGBKeyboardBacklightController>();
        builder.Register<SensorsController>();
        builder.Register<SensorsControllerV1>(true);
        builder.Register<SensorsControllerV2>(true);
        builder.Register<SensorsControllerV3>(true);
        builder.Register<SmartFnLockController>();
        builder.Register<SpectrumKeyboardBacklightController>();
        builder.Register<WindowsPowerModeController>();
        builder.Register<WindowsPowerPlanController>();

        builder.Register<UpdateChecker>();
        builder.Register<WarrantyChecker>();

        builder.Register<PackageDownloaderFactory>();
        builder.Register<PCSupportPackageDownloader>();
        builder.Register<VantagePackageDownloader>();

        builder.Register<HWiNFOIntegration>();

        builder.Register<SunriseSunset>();
    }
}
