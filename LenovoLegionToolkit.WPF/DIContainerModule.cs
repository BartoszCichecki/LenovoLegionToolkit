using Autofac;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF
{
    public class DIContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<ThemeManager>();
        }
    }
}
