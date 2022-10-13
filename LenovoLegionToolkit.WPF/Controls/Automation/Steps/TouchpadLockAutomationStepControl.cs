using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class TouchpadLockAutomationStepControl : AbstractComboBoxAutomationStepCardControl<TouchpadLockState>
    {
        public TouchpadLockAutomationStepControl(IAutomationStep<TouchpadLockState> step) : base(step)
        {
            Icon = SymbolRegular.Tablet24;
            Title = Resource.TouchpadLockAutomationStepControl_Title;
            Subtitle = Resource.TouchpadLockAutomationStepControl_Message;
        }
    }
}
