using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class TouchpadLockControl : AbstractToggleDashboardCardControl<TouchpadLockState>
    {
        protected override TouchpadLockState OnState => TouchpadLockState.On;

        protected override TouchpadLockState OffState => TouchpadLockState.Off;

        public TouchpadLockControl()
        {
            Icon = SymbolRegular.Tablet24;
            Title = "Touchpad Lock";
            Subtitle = "Disable touchpad.";
        }
    }
}
