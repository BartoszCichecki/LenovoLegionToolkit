using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class SpectrumKeyboardBacklightBrightnessAutomationStepControl : AbstractComboBoxAutomationStepCardControl<int>
    {
        public SpectrumKeyboardBacklightBrightnessAutomationStepControl(IAutomationStep<int> step) : base(step)
        {
            Icon = SymbolRegular.BrightnessHigh24;
            Title = "Keyboard backlight brightness";
            Subtitle = "Adjust keyboard backlight brightness.";
        }

        protected override string ComboBoxItemDisplayName(int value) => value == 0 ? "Off" : base.ComboBoxItemDisplayName(value);
    }
}