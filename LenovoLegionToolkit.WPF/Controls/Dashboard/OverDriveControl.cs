using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class OverDriveControl : AbstractToggleDashboardCardControl<OverDriveState>
    {
        protected override OverDriveState OnState => OverDriveState.On;

        protected override OverDriveState OffState => OverDriveState.Off;

        public OverDriveControl()
        {
            Icon = SymbolRegular.TopSpeed24;
            Title = "Over Drive";
            Subtitle = "Improve response time of the built-in display.";
        }
    }
}
