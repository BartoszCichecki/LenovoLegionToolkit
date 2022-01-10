using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LenovoLegionToolkit.WPF.Controls
{
    public abstract class RefreshingControl : UserControl
    {
        protected bool IsRefreshing => _refreshTask != null;

        private Task _refreshTask;

        public RefreshingControl()
        {
            IsEnabled = false;

            Loaded += RefreshingControl_Loaded;
            IsVisibleChanged += RefreshingControl_IsVisibleChanged;
        }

        private async void RefreshingControl_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshAsync();
        }

        private async void RefreshingControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        protected async Task RefreshAsync()
        {
            var exceptions = false;

            try
            {
                IsEnabled = false;

                if (_refreshTask == null)
                    _refreshTask = OnRefreshAsync();
                await _refreshTask;
            }
            catch
            {
                exceptions = true;
            }
            finally
            {
                _refreshTask = null;

                if (exceptions)
                    Visibility = Visibility.Collapsed;
                else
                    IsEnabled = true;
            }
        }

        protected abstract Task OnRefreshAsync();
    }
}
