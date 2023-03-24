using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Controls;

public abstract class AbstractRefreshingControl : UserControl
{
    private Task? _refreshTask;

    protected bool IsRefreshing => _refreshTask is not null;

    protected virtual bool DisablesWhileRefreshing => true;

    protected AbstractRefreshingControl()
    {
        IsEnabled = false;

        Loaded += RefreshingControl_Loaded;
        IsVisibleChanged += RefreshingControl_IsVisibleChanged;
    }

    private void RefreshingControl_Loaded(object sender, RoutedEventArgs e)
    {
        OnFinishedLoading();
    }

    protected abstract void OnFinishedLoading();

    private async void RefreshingControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
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