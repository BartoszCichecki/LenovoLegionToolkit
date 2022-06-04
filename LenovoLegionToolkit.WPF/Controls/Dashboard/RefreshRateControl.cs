using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class RefreshRateControl : AbstractComboBoxCardControl<RefreshRate>
    {
        private readonly DisplayConfigurationListener _listener = IoCContainer.Resolve<DisplayConfigurationListener>();

        public RefreshRateControl()
        {
            Icon = SymbolRegular.Laptop24;
            Title = "Refresh rate";
            Subtitle = "Change refresh rate of the built-in display.";

            _listener.Changed += Listener_Changed;
        }

        private void Listener_Changed(object? sender, System.EventArgs e) => Dispatcher.Invoke(async () =>
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        });
    }
}
