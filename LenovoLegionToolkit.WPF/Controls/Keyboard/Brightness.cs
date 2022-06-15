using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Keyboard
{
    public class KeyboardBrightnessControl : AbstractComboBoxCardControl<KeyboardBrightnessState>
    {
        public KeyboardBrightnessControl()
        {

            Icon = SymbolRegular.BrightnessHigh24;
            Title = "Keyboard RGB Brightness";
            Subtitle = "Choose the Keyboard Color Brightness";
        }
    }
}
