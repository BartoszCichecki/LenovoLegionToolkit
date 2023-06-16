using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class PortsBacklightAutomationStepControl : AbstractComboBoxAutomationStepCardControl<PortsBacklightState>
{
    public PortsBacklightAutomationStepControl(IAutomationStep<PortsBacklightState> step) : base(step)
    {
        Icon = SymbolRegular.UsbPlug24;
        Title = Resource.PortsBacklightAutomationStepControl_Title;
        Subtitle = Resource.PortsBacklightAutomationStepControl_Message;
    }
}
