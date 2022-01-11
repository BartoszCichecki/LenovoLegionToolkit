using System;
using Autofac;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal class Container
    {
        private static IContainer? _container;

        public static void Initialize()
        {
            var cb = new ContainerBuilder();

            cb.RegisterType<ThemeManager>();

            // Lib
            cb.RegisterType<AlwaysOnUsbFeature>();
            cb.RegisterType<BatteryFeature>();
            cb.RegisterType<FlipToStartFeature>();
            cb.RegisterType<FnLockFeature>();
            cb.RegisterType<HybridModeFeature>();
            cb.RegisterType<OverDriveFeature>();
            cb.RegisterType<PowerModeFeature>();
            cb.RegisterType<RefreshRateFeature>();
            cb.RegisterType<TouchpadLockFeature>();
            cb.RegisterType<PowerModeListener>();
            cb.RegisterType<GPUController>();

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
