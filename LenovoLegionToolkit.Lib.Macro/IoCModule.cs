using Autofac;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Macro.Utils;

namespace LenovoLegionToolkit.Lib.Macro;

public class IoCModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<MacroSettings>();
        builder.Register<MacroController>();
    }
}
