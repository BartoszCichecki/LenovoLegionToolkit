using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class RGBKeyboardBacklightAutomationStepControl : AbstractComboBoxAutomationStepCardControl<RGBKeyboardBacklightSelectedPreset>
    {
        public RGBKeyboardBacklightAutomationStepControl(IAutomationStep<RGBKeyboardBacklightSelectedPreset> step) : base(step)
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Keyboard backlight";
            Subtitle = "Adjust keyboard backlight.";
        }
    }
}
