using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class DeactivateGPUAutomationStepControl : AbstractComboBoxAutomationStepCardControl<DeactivateGPUAutomationStepState>
{
    public DeactivateGPUAutomationStepControl(DeactivateGPUAutomationStep step) : base(step)
    {
        Icon = SymbolRegular.DeveloperBoard24;
        Title = Resource.DeactivateGPUAutomationStepControl_Title;
        Subtitle = Resource.DeactivateGPUAutomationStepControl_Message;
    }
}