using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Keyboard
{
    public class Zone4Color : AbstractColorPickerCardControl<KeyboardSpeedState>
    {
        public Zone4Color()
        {
            Icon = SymbolRegular.Color24;
            Title = "Zone4 Color";
            Subtitle = "Choose the Color of Zone 4";
        }
    }
}
