using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    internal class WhiteKeyboardBacklightControl : AbstractComboBoxCardControl<WhiteKeyboardBacklightStatus>
    {
        public WhiteKeyboardBacklightControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Keyboard Backlight";
            Subtitle = "Adjust keyboard backlight brightness.\nYou can change brightness using shortcut Fn+Space";
        }
    }
}
