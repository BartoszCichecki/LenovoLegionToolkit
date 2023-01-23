using Autofac;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Settings;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF;

public class IoCModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<SpectrumScreenCapture>();

        builder.Register<ThemeManager>().AutoActivate();
        builder.Register<NotificationsManager>().AutoActivate();

        builder.Register<DashboardSettings>();
    }
}