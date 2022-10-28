using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.White
{
    public class WhiteKeyboardBacklightControl : AbstractComboBoxFeatureCardControl<WhiteKeyboardBacklightState>
    {
        private readonly LightingChangeListener _listener = IoCContainer.Resolve<LightingChangeListener>();

        public WhiteKeyboardBacklightControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = Resource.WhiteKeyboardBacklightControl_Title;
            Subtitle = Resource.WhiteKeyboardBacklightControl_Message;

            _listener.Changed += ListenerChanged;
        }

        private void ListenerChanged(object? sender, LightingChangeState e) => Dispatcher.Invoke(async () =>
        {
            if (!IsLoaded || !IsVisible)
                return;

            if (e != LightingChangeState.KeyboardBacklight)
                return;

            await RefreshAsync();
        });
    }
}
