using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class DeactivateGPUAutomationStepControl : AbstractComboBoxAutomationStepCardControl<DeactivateGPUAutomationStepState>
{
    public DeactivateGPUAutomationStepControl(DeactivateGPUAutomationStep step) : base(step)
    {
        Icon = SymbolRegular.DeveloperBoard24.GetIcon();
        Title = Resource.DeactivateGPUAutomationStepControl_Title;
        Subtitle = Resource.DeactivateGPUAutomationStepControl_Message;
    }
}
