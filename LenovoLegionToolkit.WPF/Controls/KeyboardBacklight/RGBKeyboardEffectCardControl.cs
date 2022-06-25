using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public class RGBKeyboardEffectCardControl : AbstractComboBoxKeyboardBacklightCardControl<RGBKeyboardEffect>
    {
        public RGBKeyboardEffectCardControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Effect";
        }
    }
}
