using Autofac;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;

namespace LenovoLegionToolkit.WPF
{
    internal class Container
    {
        private static IContainer _container;

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
            cb.RegisterType<PowerModeFeature>()
                .OnActivated(async e => await e.Instance.EnsureCorrectPowerPlanIsSetAsync());
            cb.RegisterType<RefreshRateFeature>();
            cb.RegisterType<TouchpadLockFeature>();
            cb.RegisterType<PowerModeListener>();

            _container = cb.Build();
        }

        public static T Resolve<T>() => _container.Resolve<T>();
    }
}
