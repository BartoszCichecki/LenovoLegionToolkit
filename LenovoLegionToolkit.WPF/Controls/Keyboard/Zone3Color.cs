using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Keyboard
{
    public class Zone3Color : AbstractColorPickerCardControl<KeyboardSpeedState>
    {
        public Zone3Color()
        {
            Icon = SymbolRegular.Color24;
            Title = "Zone3 Color";
            Subtitle = "Choose the Color of Zone 3";
        }
    }
}
