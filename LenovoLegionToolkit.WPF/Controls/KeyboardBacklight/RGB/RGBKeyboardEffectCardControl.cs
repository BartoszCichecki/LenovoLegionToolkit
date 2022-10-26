using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.RGB
{
    public class RGBKeyboardEffectCardControl : AbstractComboBoxRGBKeyboardCardControl<RGBKeyboardEffect>
    {
        public RGBKeyboardEffectCardControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = Resource.RGBKeyboardEffectCardControl_Title;
        }
    }
}
