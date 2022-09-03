using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class AlwaysOnUSBControl : AbstractComboBoxFeatureCardControl<AlwaysOnUSBState>
    {
        public AlwaysOnUSBControl()
        {
            Icon = SymbolRegular.UsbStick24;
            Title = "Always on USB";
            Subtitle = "Charge USB devices, when the computer is off or in sleep or hibernation mode.";
        }
    }
}
