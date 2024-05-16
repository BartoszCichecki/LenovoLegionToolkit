using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class MacroAutomationStepControl : AbstractComboBoxAutomationStepCardControl<MacroAutomationStepState>
{
    public MacroAutomationStepControl(IAutomationStep<MacroAutomationStepState> step) : base(step)
    {
        Icon = SymbolRegular.ReceiptPlay24;
        Title = Resource.MacroAutomationStepControl_Title;
        Subtitle = Resource.MacroAutomationStepControl_Message;
    }
}
