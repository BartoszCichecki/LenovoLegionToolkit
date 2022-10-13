using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public class RGBKeyboardBrightnessCardControl : AbstractComboBoxKeyboardBacklightCardControl<RGBKeyboardBrightness>
    {
        public RGBKeyboardBrightnessCardControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = Resource.RGBKeyboardBrightnessCardControl_Brightness;
        }
    }
}
