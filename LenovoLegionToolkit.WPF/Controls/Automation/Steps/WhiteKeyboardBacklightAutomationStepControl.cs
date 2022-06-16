using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    internal class WhiteKeyboardBacklightAutomationStepControl : AbstractComboBoxAutomationStepCardControl<WhiteKeyboardBacklightState>
    {
        public WhiteKeyboardBacklightAutomationStepControl(IAutomationStep<WhiteKeyboardBacklightState> step) : base(step)
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Keyboard backlight";
            Subtitle = "Adjust keyboard backlight brightness.";
        }
    }
}
