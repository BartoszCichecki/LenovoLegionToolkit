using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public class RGBKeyboardSpeedCardControl : AbstractComboBoxKeyboardBacklightCardControl<RBGKeyboardSpeed>
    {
        public RGBKeyboardSpeedCardControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Speed";
        }
    }
}
