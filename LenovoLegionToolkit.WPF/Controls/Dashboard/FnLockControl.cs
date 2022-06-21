using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class FnLockControl : AbstractToggleDashboardCardControl<FnLockState>
    {
        protected override FnLockState OnState => FnLockState.On;

        protected override FnLockState OffState => FnLockState.Off;

        public FnLockControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = "Fn Lock";
            Subtitle = "Use secondary functions of F1-F12 keys without holding Fn key.";
        }
    }
}
