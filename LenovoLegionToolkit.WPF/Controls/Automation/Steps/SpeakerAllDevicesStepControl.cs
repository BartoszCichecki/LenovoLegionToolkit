using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class SpeakerAllDevicesStepControl : AbstractComboBoxAutomationStepCardControl<SpeakerAllDevicesState>
{
    public SpeakerAllDevicesStepControl(IAutomationStep<SpeakerAllDevicesState> step) : base(step)
    {
        Icon = SymbolRegular.Speaker224;
        Title = Resource.SpeakerAllDevicesStepControl_Title;
        Subtitle = Resource.SpeakerAllDevicesStepControl_Message;
    }
}
