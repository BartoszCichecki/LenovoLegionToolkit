using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class AlwaysOnUsbAutomationStepControl : AbstractComboBoxAutomationStepCardControl<AlwaysOnUSBState>
    {
        public AlwaysOnUsbAutomationStepControl(IAutomationStep<AlwaysOnUSBState> step) : base(step)
        {
            Icon = SymbolRegular.UsbStick24;
            Title = "Always on USB";
            Subtitle = "Charge USB devices, when the computer is\noff or in sleep or hibernation mode.";
        }
    }
}
