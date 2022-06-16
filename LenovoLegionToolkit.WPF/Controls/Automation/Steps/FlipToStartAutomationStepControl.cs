using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class FlipToStartAutomationStepControl : AbstractComboBoxAutomationStepCardControl<FlipToStartState>
    {
        public FlipToStartAutomationStepControl(IAutomationStep<FlipToStartState> step) : base(step)
        {
            Icon = SymbolRegular.Power24;
            Title = "Flip To Start";
            Subtitle = "Turn on the laptop when you open the lid.";
        }
    }
}
