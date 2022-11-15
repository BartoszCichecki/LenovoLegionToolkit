using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class SpectrumKeyboardBacklightProfileAutomationStepControl : AbstractComboBoxAutomationStepCardControl<int>
    {
        public SpectrumKeyboardBacklightProfileAutomationStepControl(IAutomationStep<int> step) : base(step)
        {
            Icon = SymbolRegular.BrightnessHigh24;
            Title = Resource.SpectrumKeyboardBacklightProfileAutomationStepControl_Title;
            Subtitle = Resource.SpectrumKeyboardBacklightProfileAutomationStepControl_Message;
        }
    }
}