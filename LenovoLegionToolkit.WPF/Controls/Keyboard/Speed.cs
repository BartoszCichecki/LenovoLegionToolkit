using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Keyboard
{
    public class KeyboardSpeedControl : AbstractComboBoxCardControl<KeyboardSpeedState>
    {
        public KeyboardSpeedControl()
        {
            Icon = SymbolRegular.TopSpeed24;
            Title = "Keyboard Speed";
            Subtitle = "Choose the Keyboard Speed";
        }
    }
}
