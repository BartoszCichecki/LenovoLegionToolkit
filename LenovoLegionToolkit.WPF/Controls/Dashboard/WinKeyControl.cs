using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class WinKeyControl : AbstractToggleFeatureCardControl<WinKeyState>
    {
        protected override WinKeyState OnState => WinKeyState.On;

        protected override WinKeyState OffState => WinKeyState.Off;

        public WinKeyControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Windows Key";
            Subtitle = "Enabled or disable Windows key.";
        }
    }
}
