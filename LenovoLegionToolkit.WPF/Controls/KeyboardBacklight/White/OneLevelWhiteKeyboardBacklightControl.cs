using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.White;

internal class OneLevelWhiteKeyboardBacklightControl : AbstractToggleFeatureCardControl<OneLevelWhiteKeyboardBacklightState>
{
    protected override OneLevelWhiteKeyboardBacklightState OnState => OneLevelWhiteKeyboardBacklightState.On;

    protected override OneLevelWhiteKeyboardBacklightState OffState => OneLevelWhiteKeyboardBacklightState.Off;

    public OneLevelWhiteKeyboardBacklightControl()
    {
        Icon = SymbolRegular.Keyboard24;
        Title = Resource.OneLevelWhiteKeyboardBacklightControl_Title;
        Subtitle = Resource.OneLevelWhiteKeyboardBacklightControl_Message;
    }
}
