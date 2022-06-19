using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class DelayAutomationStepControl : AbstractComboBoxAutomationStepCardControl<Delay>
    {
        public DelayAutomationStepControl(IAutomationStep<Delay> step) : base(step)
        {
            Icon = SymbolRegular.Clock24;
            Title = "Delay";
            Subtitle = "Add delay before running next step.";
        }
    }
}
