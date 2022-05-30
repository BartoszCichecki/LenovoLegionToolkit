using Autofac;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib
{
    public class DIContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<AlwaysOnUSBFeature>();
            builder.Register<BatteryFeature>();
            builder.Register<FlipToStartFeature>();
            builder.Register<FnLockFeature>();
            builder.Register<HybridModeFeature>();
            builder.Register<OverDriveFeature>();
            builder.Register<PowerModeFeature>();
            builder.Register<RefreshRateFeature>();
            builder.Register<TouchpadLockFeature>();

            builder.Register<PowerModeListener>(true);
            builder.Register<PowerAdapterListener>(true);
            builder.Register<DisplayConfigurationListener>(true);
            builder.Register<SpecialKeyListener>(true);

            builder.Register<GPUController>();
            builder.Register<CPUBoostModeController>();

            builder.Register<UpdateChecker>();
        }
    }
}
