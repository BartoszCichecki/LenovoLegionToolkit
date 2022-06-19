using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class BatteryAutomationStepControl : AbstractComboBoxAutomationStepCardControl<BatteryState>
    {
        public BatteryAutomationStepControl(IAutomationStep<BatteryState> step) : base(step)
        {
            Icon = SymbolRegular.BatteryCharge24;
            Title = "Battery Mode";
            Subtitle = "Choose how the battery is charged.";
        }
    }
}
