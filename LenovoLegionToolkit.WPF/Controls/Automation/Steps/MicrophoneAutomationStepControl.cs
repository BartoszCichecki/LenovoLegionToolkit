using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class MicrophoneAutomationStepControl : AbstractComboBoxAutomationStepCardControl<MicrophoneState>
{
    public MicrophoneAutomationStepControl(IAutomationStep<MicrophoneState> step) : base(step)
    {
        Icon = SymbolRegular.Mic24;
        Title = Resource.MicrophoneAutomationStepControl_Title;
        Subtitle = Resource.MicrophoneAutomationStepControl_Message;
    }
}