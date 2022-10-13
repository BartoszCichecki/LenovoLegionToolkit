using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class AlwaysOnUSBControl : AbstractComboBoxFeatureCardControl<AlwaysOnUSBState>
    {
        public AlwaysOnUSBControl()
        {
            Icon = SymbolRegular.UsbStick24;
            Title = Resource.AlwaysOnUSBControl_Title;
            Subtitle = Resource.AlwaysOnUSBControl_Message;
        }
    }
}
