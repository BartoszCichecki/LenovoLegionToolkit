using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class TouchpadLockAutomationStepControl : AbstractComboBoxAutomationStepCardControl<TouchpadLockState>
    {
        public TouchpadLockAutomationStepControl(IAutomationStep<TouchpadLockState> step) : base(step)
        {
            Icon = SymbolRegular.Tablet24;
            Title = "Touchpad Lock";
            Subtitle = "Disable touchpad.";
        }
    }
}
