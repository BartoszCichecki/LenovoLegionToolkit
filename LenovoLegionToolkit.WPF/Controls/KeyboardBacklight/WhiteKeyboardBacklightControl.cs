using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public class WhiteKeyboardBacklightControl : AbstractComboBoxFeatureCardControl<WhiteKeyboardBacklightState>
    {
        private readonly WhiteKeyboardBacklightListener _listener = IoCContainer.Resolve<WhiteKeyboardBacklightListener>();

        public WhiteKeyboardBacklightControl()
        {
            Icon = SymbolRegular.Keyboard24;
            Title = Resource.WhiteKeyboardBacklightControl_Title;
            Subtitle = Resource.WhiteKeyboardBacklightControl_Message;

            _listener.Changed += Listener_Changed;
        }

        private void Listener_Changed(object? sender, WhiteKeyboardBacklightChanged e) => Dispatcher.Invoke(async () =>
        {
            if (!IsLoaded || !IsVisible)
                return;

            await RefreshAsync();
        });
    }
}
