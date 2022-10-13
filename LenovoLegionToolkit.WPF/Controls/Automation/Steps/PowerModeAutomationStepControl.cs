using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class PowerModeAutomationStepControl : AbstractComboBoxAutomationStepCardControl<PowerModeState>
    {
        public PowerModeAutomationStepControl(IAutomationStep<PowerModeState> step) : base(step)
        {
            Icon = SymbolRegular.Gauge24;
            Title = Resource.PowerModeAutomationStepControl_Title;
            Subtitle = Resource.PowerModeAutomationStepControl_Message;
        }
    }
}
