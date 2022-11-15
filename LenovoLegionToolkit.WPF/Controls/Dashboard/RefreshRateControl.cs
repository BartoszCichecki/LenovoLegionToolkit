using System;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class RefreshRateControl : AbstractComboBoxFeatureCardControl<RefreshRate>
    {
        private readonly DisplayConfigurationListener _listener = IoCContainer.Resolve<DisplayConfigurationListener>();

        public RefreshRateControl()
        {
            Icon = SymbolRegular.Laptop24;
            Title = Resource.RefreshRateControl_Title;
            Subtitle = Resource.RefreshRateControl_Message;

            _listener.Changed += Listener_Changed;
        }

        protected override async Task OnRefreshAsync()
        {
            await base.OnRefreshAsync();

            Visibility = ItemsCount < 2 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Listener_Changed(object? sender, EventArgs e) => Dispatcher.Invoke(async () =>
        {
            if (IsLoaded)
                await RefreshAsync();
        });
    }
}
