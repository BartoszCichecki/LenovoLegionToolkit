using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Keyboard
{
    public class Zone2Color : AbstractColorPickerCardControl<KeyboardSpeedState>
    {
        public Zone2Color()
        {
            Icon = SymbolRegular.Color24;
            Title = "Zone2 Color";
            Subtitle = "Choose the Color of Zone 2";
        }
    }
}
