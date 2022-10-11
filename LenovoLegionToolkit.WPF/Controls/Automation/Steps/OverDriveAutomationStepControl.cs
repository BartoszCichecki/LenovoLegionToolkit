using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class OverDriveAutomationStepControl : AbstractComboBoxAutomationStepCardControl<OverDriveState>
    {
        public OverDriveAutomationStepControl(IAutomationStep<OverDriveState> step) : base(step)
        {
            Icon = SymbolRegular.TopSpeed24;
            Title = Resource.OverDriveAutomationStepControl_Title;
            Subtitle = Resource.OverDriveAutomationStepControl_Message;
        }
    }
}
