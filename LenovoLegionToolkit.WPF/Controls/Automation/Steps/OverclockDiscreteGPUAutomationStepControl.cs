using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class OverclockDiscreteGPUAutomationStepControl : AbstractComboBoxAutomationStepCardControl<OverclockDiscreteGPUAutomationStepState>
{
    public OverclockDiscreteGPUAutomationStepControl(IAutomationStep<OverclockDiscreteGPUAutomationStepState> step) : base(step)
    {
        Icon = SymbolRegular.DeveloperBoardLightning20.GetIcon();
        Title = Resource.OverclockDiscreteGPUAutomationStepControl_Title;
        Subtitle = Resource.OverclockDiscreteGPUAutomationStepControl_Message;
    }
}
