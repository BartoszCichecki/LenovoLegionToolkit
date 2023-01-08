using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class MicrophoneMuteControl : AbstractToggleFeatureCardControl<MicrophoneMuteState>
{
    private readonly DriverKeyListener _listener = IoCContainer.Resolve<DriverKeyListener>();

    protected override MicrophoneMuteState OnState => MicrophoneMuteState.On;
    protected override MicrophoneMuteState OffState => MicrophoneMuteState.Off;

    public MicrophoneMuteControl()
    {
        Icon = SymbolRegular.MicOff24;
        Title = Resource.MicrophoneMuteControl_Title;
        Subtitle = Resource.MicrophoneMuteControl_Message;

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
