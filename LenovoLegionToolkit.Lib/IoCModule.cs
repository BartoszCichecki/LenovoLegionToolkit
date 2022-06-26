using Autofac;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib
{
    public class IoCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<ApplicationSettings>();
            builder.Register<RGBKeyboardSettings>();

            builder.Register<AlwaysOnUSBFeature>();
            builder.Register<BatteryFeature>();
            builder.Register<FlipToStartFeature>();
            builder.Register<FnLockFeature>();
            builder.Register<HybridModeFeature>();
            builder.Register<OverDriveFeature>();
            builder.Register<PowerModeFeature>();
            builder.Register<RefreshRateFeature>();
            builder.Register<TouchpadLockFeature>();
            builder.Register<WhiteKeyboardBacklightFeature>();
            builder.Register<WinKeyFeature>();

            builder.Register<PowerModeListener>().AutoActivate();
            builder.Register<PowerStateListener>().AutoActivate();
            builder.Register<DisplayConfigurationListener>().AutoActivate();
            builder.Register<PowerPlanListener>().AutoActivate();
            builder.Register<ProcessListener>().AutoActivate();
            builder.Register<RGBKeyboardBacklightListener>().AutoActivate();
            builder.Register<SpecialKeyListener>().AutoActivate();
            builder.Register<WhiteKeyboardBacklightListener>().AutoActivate();
            builder.Register<WinKeyListener>().AutoActivate();

            builder.Register<GPUController>();
            builder.Register<CPUBoostModeController>();
            builder.Register<RGBKeyboardBacklightController>();

            builder.Register<Vantage>();
            builder.Register<FnKeys>();

            builder.Register<UpdateChecker>();
        }
    }
}
