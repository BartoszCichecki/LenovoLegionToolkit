using LenovoLegionToolkit.Lib;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class RefreshRateControl : AbstractComboBoxCardControl<RefreshRate>
    {
        public RefreshRateControl()
        {
            Icon = SymbolRegular.Laptop24;
            Title = "Refresh rate";
            Subtitle = "Change refresh rate of the built-in display.";
        }
    }
}
