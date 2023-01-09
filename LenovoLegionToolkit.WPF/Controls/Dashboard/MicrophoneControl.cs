using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class MicrophoneControl : AbstractToggleFeatureCardControl<MicrophoneState>
{
    private readonly DriverKeyListener _listener = IoCContainer.Resolve<DriverKeyListener>();

    protected override MicrophoneState OnState => MicrophoneState.On;
    protected override MicrophoneState OffState => MicrophoneState.Off;

    public MicrophoneControl()
    {
        Icon = SymbolRegular.Mic24;
        Title = Resource.MicrophoneControl_Title;
        Subtitle = Resource.MicrophoneControl_Message;

        _listener.Changed += Listener_Changed;
    }

    private void Listener_Changed(object? sender, DriverKey e) => Dispatcher.Invoke(async () =>
    {
        if (!IsLoaded || !IsVisible)
            return;

        if (e.HasFlag(DriverKey.Fn_F4))
            await RefreshAsync();
    });
}
