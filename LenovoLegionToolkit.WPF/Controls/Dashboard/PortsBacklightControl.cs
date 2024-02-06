﻿using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class PortsBacklightControl : AbstractToggleFeatureCardControl<PortsBacklightState>
{
    private readonly LightingChangeListener _listener = IoCContainer.Resolve<LightingChangeListener>();

    protected override PortsBacklightState OnState => PortsBacklightState.On;

    protected override PortsBacklightState OffState => PortsBacklightState.Off;

    public PortsBacklightControl()
    {
        Icon = SymbolRegular.UsbPlug24.GetIcon();
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
