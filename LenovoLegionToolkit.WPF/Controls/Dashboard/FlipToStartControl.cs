using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class FlipToStartControl : AbstractToggleFeatureCardControl<FlipToStartState>
{
    protected override FlipToStartState OnState => FlipToStartState.On;

    protected override FlipToStartState OffState => FlipToStartState.Off;

    public FlipToStartControl()
    {
        Icon = SymbolRegular.Power24.GetIcon();
        Title = Resource.FlipToStartControl_Title;
        Subtitle = Resource.FlipToStartControl_Message;
    }
}
