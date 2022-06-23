using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    internal class WhiteKeyboardBacklightControl : AbstractComboBoxDashboardCardControl<WhiteKeyboardBacklightState>
    {
        public WhiteKeyboardBacklightControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Brightness";
            Subtitle = "You can change brightness with Fn+Space";
        }
    }
}
