using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    internal class PowerModeAutomationStepControl : AbstractComboBoxAutomationStepCardControl<PowerModeState>
    {
        public PowerModeAutomationStepControl(IAutomationStep<PowerModeState> step) : base(step)
        {
            Icon = SymbolRegular.Gauge24;
            Title = "Power Mode";
            Subtitle = "Select performance mode.";
        }
    }
}
