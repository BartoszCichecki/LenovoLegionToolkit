using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Controls;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Pages;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows;

public partial class MainWindow
{
    private readonly ApplicationSettings _applicationSettings = IoCContainer.Resolve<ApplicationSettings>();
    private readonly UpdateChecker _updateChecker = IoCContainer.Resolve<UpdateChecker>();

    private readonly ContextMenuHelper _contextMenuHelper = new();

    public bool SuppressClosingEventHandler { get; set; }

    public Snackbar Snackbar => _snackbar;

    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        _title.Text += " [DEBUG]";
#else
        var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;
        if (version == new Version(0, 0, 1, 0) || version?.Build == 99)
            _title.Text += " [BETA]";
#endif


        if (Log.Instance.IsTraceEnabled)
        {
            _title.Text += " [LOGGING ENABLED]";
            _openLogIndicator.Visibility = Visibility.Visible;
        }
    }

    private void InitializeTray()
    {
        _contextMenuHelper.BringToForeground = BringToForeground;
        _contextMenuHelper.Close = App.Current.ShutdownAsync;

        _trayIcon.TrayToolTip = new StatusTrayPopup();
        _trayIcon.TrayToolTipResolved.Style = null;
        _trayIcon.TrayToolTipResolved.VerticalOffset = -8;

        _trayIcon.ToolTipText = Resource.AppName;

        _trayIcon.ContextMenu = _contextMenuHelper.ContextMenu;
        _trayIcon.TrayContextMenuOpen += (_, _) =>
        {
            if (_trayIcon.TrayToolTipResolved is { } tooltip)
                tooltip.IsOpen = false;
        };

        _trayIcon.TrayLeftMouseUp += (_, _) => BringToForeground();
    }

    private void MainWindow_SourceInitialized(object? sender, EventArgs e) => RestoreSize();

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _contentGrid.Visibility = Visibility.Hidden;

        if (!await KeyboardBacklightPage.IsSupportedAsync())
            _navigationStore.Items.Remove(_keyboardItem);

        _contextMenuHelper.SetNavigationItems(_navigationStore);

        SmartKeyHelper.Instance.BringToForeground = () => Dispatcher.Invoke(BringToForeground);

        _contentGrid.Visibility = Visibility.Visible;

        LoadDeviceInfo();
        CheckForUpdates();

        InitializeTray();
    }

    private async void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        SaveSize();

        if (SuppressClosingEventHandler)
        {
            _trayIcon.Dispose();
            return;
        }

        if (_applicationSettings.Store.MinimizeOnClose)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Minimizing...");

            WindowState = WindowState.Minimized;
            e.Cancel = true;
        }
        else
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Closing...");

            await App.Current.ShutdownAsync();
        }
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Window state changed to {WindowState}");

        switch (WindowState)
        {
            case WindowState.Minimized:
                SendToTray();
                break;
            case WindowState.Normal:
                BringToForeground();
                break;
        }
    }

    private void MainWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
        if (!IsVisible)
            return;

        CheckForUpdates();
    }

    private void OpenLogIndicator_Click(object sender, MouseButtonEventArgs e)
    {
        try
        {
            new Uri(Log.Instance.LogPath).Open();
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to open log.", ex);
        }
    }

    private void DeviceInfoIndicator_Click(object sender, RoutedEventArgs e)
    {
        var window = new DeviceInformationWindow { Owner = this };
        window.ShowDialog();
    }

    private void UpdateIndicator_Click(object sender, RoutedEventArgs e) => ShowUpdateWindow();

    private void LoadDeviceInfo()
    {
        Task.Run(Compatibility.GetMachineInformationAsync)
            .ContinueWith(mi =>
            {
                _deviceInfoIndicator.Content = mi.Result.Model;
                _deviceInfoIndicator.Visibility = Visibility.Visible;
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void CheckForUpdates()
    {
        Task.Run(_updateChecker.Check)
            .ContinueWith(updatesAvailable =>
            {
                var result = updatesAvailable.Result;
                if (result is null)
                {
                    _updateIndicator.Visibility = Visibility.Collapsed;
                    return;
                }

                var versionNumber = result.ToString(3);

                _updateIndicatorText.Text = string.Format(Resource.MainWindow_UpdateAvailableWithVersion, versionNumber);
                _updateIndicator.Visibility = Visibility.Visible;

                if (WindowState == WindowState.Minimized)
                    MessagingCenter.Publish(new Notification(NotificationType.UpdateAvailable, NotificationDuration.Long, versionNumber));
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void RestoreSize()
    {
        if (!_applicationSettings.Store.WindowSize.HasValue)
            return;

        Width = _applicationSettings.Store.WindowSize.Value.Width;
        Height = _applicationSettings.Store.WindowSize.Value.Height;

        var desktopWorkingArea = ScreenHelper.GetPrimaryDesktopWorkingArea();

        Left = (desktopWorkingArea.Width - Width) / 2 + desktopWorkingArea.Left;
        Top = (desktopWorkingArea.Height - Height) / 2 + desktopWorkingArea.Top;
    }

    private void SaveSize()
    {
        _applicationSettings.Store.WindowSize = WindowState != WindowState.Normal
            ? new(RestoreBounds.Width, RestoreBounds.Height)
            : new(Width, Height);
        _applicationSettings.SynchronizeStore();
    }

    private void BringToForeground() => WindowExtensions.BringToForeground(this);

    public void SendToTray()
    {
        Hide();
        ShowInTaskbar = false;
    }

    public void ShowUpdateWindow()
    {
        var window = new UpdateWindow { Owner = this };
        window.ShowDialog();
    }
}