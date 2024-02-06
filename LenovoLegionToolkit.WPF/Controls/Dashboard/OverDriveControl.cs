using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class OverDriveControl : AbstractToggleFeatureCardControl<OverDriveState>
{
    protected override OverDriveState OnState => OverDriveState.On;

    protected override OverDriveState OffState => OverDriveState.Off;

    public OverDriveControl()
    {
        Icon = SymbolRegular.TopSpeed24.GetIcon();
        Title = Resource.OverDriveControl_Title;
        Subtitle = Resource.OverDriveControl_Message;
    }
}
