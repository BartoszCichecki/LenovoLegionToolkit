using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Keyboard
{
    public class KeyboardEffectControl : AbstractComboBoxCardControl<KeyboardEffectState>
    {
        public KeyboardEffectControl()
        {
            Icon = SymbolRegular.TextEffects24;
            Title = "Keyboard Effect";
            Subtitle = "Choose the Keyboard Effect";
        }
    }

}
