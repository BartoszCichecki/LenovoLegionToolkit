﻿using Autofac;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.PackageDownloader;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib
{
    public class IoCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<Vantage>();
            builder.Register<FnKeys>();
            builder.Register<LegionZone>();

            builder.Register<ApplicationSettings>();
            builder.Register<RGBKeyboardSettings>();
            builder.Register<GodModeSettings>();
            builder.Register<PackageDownloaderSettings>();
            builder.Register<SunriseSunsetSettings>();

            builder.Register<AlwaysOnUSBFeature>();
            builder.Register<BatteryFeature>();
            builder.Register<FlipToStartFeature>();
            builder.Register<FnLockFeature>();
            builder.Register<HybridModeFeature>();
            builder.Register<GSyncFeature>();
            builder.Register<IGPUModeFeature>();
            builder.Register<OverDriveFeature>();
            builder.Register<PowerModeFeature>();
            builder.Register<RefreshRateFeature>();
            builder.Register<TouchpadLockFeature>();
            builder.Register<WhiteKeyboardBacklightFeature>();
            builder.Register<WinKeyFeature>();

            builder.Register<PowerStateListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();
            builder.Register<PowerModeListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();
            builder.Register<DisplayConfigurationListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();
            builder.Register<DriverKeyListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();
            builder.Register<PowerPlanListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();
            builder.Register<RGBKeyboardBacklightListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();
            builder.Register<SpecialKeyListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();
            builder.Register<WhiteKeyboardBacklightListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();
            builder.Register<WinKeyListener>()
                .OnActivating(e => e.Instance.StartAsync().AsValueTask())
                .AutoActivate();

            builder.Register<GPUController>();
            builder.Register<CPUBoostModeController>();
            builder.Register<RGBKeyboardBacklightController>();
            builder.Register<GodModeController>();
            builder.Register<DisplayBrightnessController>();

            builder.Register<UpdateChecker>();
            builder.Register<WarrantyChecker>();

            builder.Register<PCSupportPackageDownloader>();
            builder.Register<CommercialPackageDownloader>();
            builder.Register<PackageDownloaderFactory>();

            builder.Register<SunriseSunset>();
        }
    }
}
