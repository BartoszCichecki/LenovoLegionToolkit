using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public class RGBKeyboardEffectCardControl : AbstractComboBoxKeyboardBacklightCardControl<RGBKeyboardEffect>
    {
        public RGBKeyboardEffectCardControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = Resource.RGBKeyboardEffectCardControl_Title;
        }
    }
}
