using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public abstract class AbstractRefreshingControl : UserControl
    {
        private readonly TaskCompletionSource _finishedLoadingTaskCompletionSource = new();

        private Task? _refreshTask;

        protected bool IsRefreshing => _refreshTask is not null;

        protected virtual bool DisablesWhileRefreshing => true;

        public Task FinishedLoadingTask => _finishedLoadingTaskCompletionSource.Task;

        protected AbstractRefreshingControl()
        {
            IsEnabled = false;

            Loaded += RefreshingControl_Loaded;
            IsVisibleChanged += RefreshingControl_IsVisibleChanged;
        }

        private async void RefreshingControl_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingTask = Task.Delay(500);
            await RefreshAsync();
            await loadingTask;
            _ = _finishedLoadingTaskCompletionSource.TrySetResult();
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
                if (DisablesWhileRefreshing)
                    IsEnabled = false;

                _refreshTask ??= OnRefreshAsync();
                await _refreshTask;
            }
            catch (Exception ex)
            {
                exceptions = true;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Exception when refreshing control. [feature={GetType().Name}]", ex);
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
