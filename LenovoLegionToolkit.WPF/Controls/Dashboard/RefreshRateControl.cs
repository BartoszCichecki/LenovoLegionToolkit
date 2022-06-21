using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class RefreshRateControl : AbstractComboBoxDashboardCardControl<RefreshRate>
    {
        private readonly DisplayConfigurationListener _listener = IoCContainer.Resolve<DisplayConfigurationListener>();

        public RefreshRateControl()
        {
            Icon = SymbolRegular.Laptop24;
            Title = "Refresh rate";
            Subtitle = "Change refresh rate of the built-in display.";

            _listener.Changed += Listener_Changed;
        }

        protected override async Task OnRefreshAsync()
        {
            await base.OnRefreshAsync();

            if (_comboBox.Items.Count < 1)
                Visibility = Visibility.Collapsed;
            else
                Visibility = Visibility.Visible;
        }

        private void Listener_Changed(object? sender, System.EventArgs e) => Dispatcher.Invoke(async () =>
        {
            if (IsLoaded)
                await RefreshAsync();
        });
    }
}
