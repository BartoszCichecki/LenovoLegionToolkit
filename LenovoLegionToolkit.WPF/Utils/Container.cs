using System;
using Autofac;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal class Container
    {
        private static IContainer? _container;

        public static void Initialize()
        {
            var cb = new ContainerBuilder();

            cb.RegisterType<ThemeManager>().SingleInstance();

            // Lib
            cb.RegisterType<AlwaysOnUsbFeature>().SingleInstance();
            cb.RegisterType<BatteryFeature>().SingleInstance();
            cb.RegisterType<FlipToStartFeature>().SingleInstance();
            cb.RegisterType<FnLockFeature>().SingleInstance();
            cb.RegisterType<HybridModeFeature>().SingleInstance();
            cb.RegisterType<OverDriveFeature>().SingleInstance();
            cb.RegisterType<PowerModeFeature>().SingleInstance();
            cb.RegisterType<RefreshRateFeature>().SingleInstance();
            cb.RegisterType<TouchpadLockFeature>().SingleInstance();
            cb.RegisterType<PowerModeListener>().SingleInstance();
            cb.RegisterType<GPUController>().SingleInstance();
            cb.RegisterType<CPUBoostModeController>().SingleInstance();
            cb.RegisterType<UpdateChecker>().SingleInstance();

            _container = cb.Build();
        }

        public static T Resolve<T>() where T : notnull
        {
            if (_container == null)
                throw new InvalidOperationException("Container must be initialized first");
            return _container.Resolve<T>();
        }
    }
}
