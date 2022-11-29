using Autofac;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF;

public class IoCModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<ThemeManager>();
        builder.Register<SpectrumScreenCapture>();
        builder.Register<NotificationsManager>().AutoActivate();
    }
}