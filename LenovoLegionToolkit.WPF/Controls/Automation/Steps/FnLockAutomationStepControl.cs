using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class FnLockAutomationStepControl : AbstractComboBoxAutomationStepCardControl<FnLockState>
    {
        public FnLockAutomationStepControl(IAutomationStep<FnLockState> step) : base(step)
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Fn Lock";
            Subtitle = "Use secondary functions of F1-F12 keys without holding Fn key.";
        }
    }
}
