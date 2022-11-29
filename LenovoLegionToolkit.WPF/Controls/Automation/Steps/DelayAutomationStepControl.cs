using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class DelayAutomationStepControl : AbstractComboBoxAutomationStepCardControl<Delay>
{
    public DelayAutomationStepControl(IAutomationStep<Delay> step) : base(step)
    {
        Icon = SymbolRegular.Clock24;
        Title = Resource.DelayAutomationStepControl_Title;
        Subtitle = Resource.DelayAutomationStepControl_Message;
    }
}