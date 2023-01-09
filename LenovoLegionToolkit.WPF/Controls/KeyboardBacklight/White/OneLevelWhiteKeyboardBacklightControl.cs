using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.White;

internal class OneLevelWhiteKeyboardBacklightControl : AbstractToggleFeatureCardControl<OneLevelWhiteKeyboardBacklightState>
{
    private readonly DriverKeyListener _listener = IoCContainer.Resolve<DriverKeyListener>();

    protected override OneLevelWhiteKeyboardBacklightState OnState => OneLevelWhiteKeyboardBacklightState.On;

    protected override OneLevelWhiteKeyboardBacklightState OffState => OneLevelWhiteKeyboardBacklightState.Off;

    public OneLevelWhiteKeyboardBacklightControl()
    {
        Icon = SymbolRegular.Keyboard24;
        Title = "Backlight";
        Subtitle = "You can turn backlight on or off with Fn+Space";

        _listener.Changed += ListenerChanged;
    }

    private void ListenerChanged(object? sender, DriverKey e) => Dispatcher.Invoke(async () =>
    {
        if (!IsLoaded || !IsVisible)
            return;

        if (e.HasFlag(DriverKey.Fn_Space))
            await RefreshAsync();
    });
}
