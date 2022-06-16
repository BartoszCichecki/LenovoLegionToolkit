using System;
using Autofac;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class Container
    {
        private static IContainer? _container;

        public static void Initialize()
        {
            var cb = new ContainerBuilder();

            // Features
            cb.Register<AlwaysOnUSBFeature>();
            cb.Register<BatteryFeature>();
            cb.Register<FlipToStartFeature>();
            cb.Register<FnLockFeature>();
            cb.Register<HybridModeFeature>();
            cb.Register<OverDriveFeature>();
            cb.Register<PowerModeFeature>();
            cb.Register<RefreshRateFeature>();
            cb.Register<TouchpadLockFeature>();
            cb.Register<KeyboardEffectFeature>();
            cb.Register<KeyboardBrightnessFeature>();
            cb.Register<KeyboardSpeedFeature>();
            cb.Register<KeyboardColorZone>();
            // Listeners
            cb.Register<PowerModeListener>();
            cb.Register<PowerAdapterListener>();
            cb.Register<DisplayConfigurationListener>();
            cb.Register<SpecialKeyListener>();
            cb.Register<PowerPlanListener>();

            // Controllers
            cb.Register<GPUController>();
            cb.Register<CPUBoostModeController>();

            // Utils
            cb.Register<UpdateChecker>();
            cb.Register<ThemeManager>();

            _container = cb.Build();
        }

        public static T Resolve<T>() where T : notnull
        {
            if (_container == null)
                throw new InvalidOperationException("Container must be initialized first");
            return _container.Resolve<T>();
        }
    }

    static class ContainerBuilderExtensions
    {
        public static void Register<T>(this ContainerBuilder cb) where T : notnull
        {
            cb.RegisterType<T>().AsSelf().AsImplementedInterfaces().SingleInstance();
        }
    }
}
