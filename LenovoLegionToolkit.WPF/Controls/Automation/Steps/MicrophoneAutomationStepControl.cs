using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class MicrophoneMuteAutomationStepControl : AbstractComboBoxAutomationStepCardControl<MicrophoneState>
{
    public MicrophoneMuteAutomationStepControl(IAutomationStep<MicrophoneState> step) : base(step)
    {
        Icon = SymbolRegular.MicOff24;
        Title = Resource.MicrophoneMuteAutomationStepControl_Title;
        Subtitle = Resource.MicrophoneMuteAutomationStepControl_Message;
    }
}