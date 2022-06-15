using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Keyboard
{
    public class Zone1Color : AbstractColorPickerCardControl<KeyboardSpeedState>
    {
        public Zone1Color()
        {
            Icon = SymbolRegular.Color24;
            Title = "Zone1 Color";
            Subtitle = "Choose the Color of Zone 1";
        }
    }
}
