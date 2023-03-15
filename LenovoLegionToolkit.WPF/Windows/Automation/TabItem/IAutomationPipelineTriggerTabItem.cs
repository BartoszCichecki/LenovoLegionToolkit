using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItem;

public interface IAutomationPipelineTriggerTabItem<out T> where T : IAutomationPipelineTrigger
{
    T GetTrigger();
}
