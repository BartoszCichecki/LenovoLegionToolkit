using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class OneLevelWhiteKeyboardBacklightAutomationStepControl : AbstractComboBoxAutomationStepCardControl<OneLevelWhiteKeyboardBacklightState>
{
    public OneLevelWhiteKeyboardBacklightAutomationStepControl(IAutomationStep<OneLevelWhiteKeyboardBacklightState> step) : base(step)
    {
        Icon = SymbolRegular.Keyboard24;
        Title = Resource.OneLevelWhiteKeyboardBacklightAutomationStepControl_Title;
        Subtitle = Resource.OneLevelWhiteKeyboardBacklightAutomationStepControl_Message;
    }
}