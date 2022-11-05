using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class SpectrumKeyboardBacklightProfileAutomationStepControl : AbstractComboBoxAutomationStepCardControl<int>
    {
        public SpectrumKeyboardBacklightProfileAutomationStepControl(IAutomationStep<int> step) : base(step)
        {
            Icon = SymbolRegular.BrightnessHigh24;
            Title = "Keyboard backlight profile";
            Subtitle = "Adjust keyboard backlight profile.";
        }
    }
}