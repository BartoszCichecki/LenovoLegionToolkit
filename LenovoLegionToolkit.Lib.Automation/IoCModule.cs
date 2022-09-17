using Autofac;
using LenovoLegionToolkit.Lib.Automation.Listeners;
using LenovoLegionToolkit.Lib.Automation.Utils;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.Automation
{
    public class IoCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<AutomationSettings>();

            builder.Register<PowerStateAutomationListener>(true);
            builder.Register<PowerModeAutomationListener>(true);
            builder.Register<ProcessAutomationListener>(true);
            builder.Register<TimeAutomationListener>(true);

            builder.Register<AutomationProcessor>();
        }
    }
}
