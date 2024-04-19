using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class SpeakerSystemDefaultDeviceStepControl : AbstractComboBoxAutomationStepCardControl<SpeakerSystemDefaultDeviceState>
{
    public SpeakerSystemDefaultDeviceStepControl(IAutomationStep<SpeakerSystemDefaultDeviceState> step) : base(step)
    {
        Icon = SymbolRegular.Speaker224;
        Title = Resource.SpeakerSystemDefaultDeviceStepControl_Title;
        Subtitle = Resource.SpeakerSystemDefaultDeviceStepControl_Message;
    }
}
