using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Keyboard
{
    public class Zone1Color : AbstractColorPickerCardControl<KeyboardColorState>
    {
        public Zone1Color()
        {
            Icon = SymbolRegular.Color24;
            Title = "Zone1";
            Subtitle = "Choose the Color of Zone 1";
        }
    }
}
