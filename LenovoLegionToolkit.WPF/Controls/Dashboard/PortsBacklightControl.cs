using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class PortsBacklightControl : AbstractToggleFeatureCardControl<PortsBacklightState>
{
    private readonly LightingChangeListener _listener = IoCContainer.Resolve<LightingChangeListener>();

    protected override PortsBacklightState OnState => PortsBacklightState.On;

    protected override PortsBacklightState OffState => PortsBacklightState.Off;

    public PortsBacklightControl()
    {
        Icon = SymbolRegular.Lightbulb24;
        Title = Resource.PortsBacklightControl_Title;
        Subtitle = Resource.PortsBacklightControl_Message;

        _listener.Changed += Listener_Changed;
    }

    private void Listener_Changed(object? sender, LightingChangeState e) => Dispatcher.Invoke(async () =>
    {
        if (e != LightingChangeState.Ports)
            return;

        await RefreshAsync();
    });
}
