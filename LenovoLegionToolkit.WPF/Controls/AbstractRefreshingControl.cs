using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public abstract class AbstractRefreshingControl : UserControl
    {
        protected bool IsRefreshing => _refreshTask != null;

        private Task? _refreshTask;

        public AbstractRefreshingControl()
        {
            IsEnabled = false;

            Loaded += RefreshingControl_Loaded;
            IsVisibleChanged += RefreshingControl_IsVisibleChanged;
        }

        private async void RefreshingControl_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingTask = Task.Delay(250);
            await RefreshAsync();
            await loadingTask;
            OnFinishedLoading();
        }

        protected abstract void OnFinishedLoading();

        private async void RefreshingControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        protected async Task RefreshAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Refreshing control... [feature={GetType().Name}]");

            var exceptions = false;

            try
            {
                IsEnabled = false;

                if (_refreshTask == null)
                    _refreshTask = OnRefreshAsync();
                await _refreshTask;
            }
            catch (Exception ex)
            {
                exceptions = true;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Exception when refreshing control. [feature={GetType().Name}, ex={ex.Demystify()}]");
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
