using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.RGB;

public class RGBKeyboardBacklightBrightnessCardControl : AbstractComboBoxRGBKeyboardCardControl<RGBKeyboardBacklightBrightness>
{
    public RGBKeyboardBacklightBrightnessCardControl()
    {
        Icon = SymbolRegular.Keyboard24;
        Title = Resource.RGBKeyboardBacklightBrightnessCardControl_Brightness;
    }
}