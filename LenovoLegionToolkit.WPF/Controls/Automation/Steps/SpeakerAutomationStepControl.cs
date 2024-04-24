using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class SpeakerAutomationStepControl : AbstractComboBoxAutomationStepCardControl<SpeakerState>
{
    public SpeakerAutomationStepControl(IAutomationStep<SpeakerState> step) : base(step)
    {
        Icon = SymbolRegular.Speaker224;
        Title = Resource.SpeakerAutomationStepControl_Title;
        Subtitle = Resource.SpeakerAutomationStepControl_Message;
    }
}
