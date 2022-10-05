using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Pages;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Controls;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.WPF.Windows
{
    public partial class MainWindow
    {
        private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();
        private readonly FnKeys _fnKeys = IoCContainer.Resolve<FnKeys>();
        private readonly SpecialKeyListener _specialKeyListener = IoCContainer.Resolve<SpecialKeyListener>();
        private readonly UpdateChecker _updateChecker = IoCContainer.Resolve<UpdateChecker>();

        public Snackbar Snackbar => _snackbar;

        private SystemEventInterceptor? _systemEventInterceptor;
        private NotifyIcon? _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            SourceInitialized += MainWindow_SourceInitialized;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            IsVisibleChanged += MainWindow_IsVisibleChanged;
            StateChanged += MainWindow_StateChanged;

            if (Assembly.GetEntryAssembly()?.GetName()?.Version == new Version(0, 0, 1, 0))
                _title.Text += " [BETA]";

#if DEBUG
            _title.Text += " [DEBUG]";
#endif

            if (Log.Instance.IsTraceEnabled)
            {
                _title.Text += " [LOGGING ENABLED]";
                _openLogIndicator.Visibility = Visibility.Visible;
            }

            _specialKeyListener.Changed += SpecialKeyListener_Changed;
        }

        private void InitializeTray()
        {
            _notifyIcon?.Unregister();

            ContextMenuHelper.Instance.BringToForegroundAction = BringToForeground;

            var notifyIcon = new NotifyIcon
            {
                TooltipText = "Lenovo Legion Toolkit",
                Icon = ImageSourceExtensions.ApplicationIcon(),
                FocusOnLeftClick = false,
                MenuOnRightClick = true,
                Menu = ContextMenuHelper.Instance.ContextMenu,
            };
            notifyIcon.LeftClick += NotifyIcon_LeftClick;
            notifyIcon.Register();

            _notifyIcon = notifyIcon;
        }

        private void SpecialKeyListener_Changed(object? sender, SpecialKey e) => Dispatcher.Invoke(async () =>
        {
            if (e != SpecialKey.Fn_F9)
                return;

            if (await _fnKeys.GetStatusAsync() == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring Fn+F9 FnKeys are enabled.");

                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bringing to foreground after Fn+F9.");

            BringToForeground();
        });

        private async void MainWindow_SourceInitialized(object? sender, EventArgs args)
        {
            var igpuModeFeature = IoCContainer.Resolve<IGPUModeFeature>();

            await igpuModeFeature.NotifyAsync();

            var systemEventInterceptor = new SystemEventInterceptor(this);
            systemEventInterceptor.OnTaskbarCreated += (_, _) => InitializeTray();
            systemEventInterceptor.OnDisplayDeviceArrival += (_, _) => Dispatcher.BeginInvoke(async () => await igpuModeFeature.NotifyAsync());
            _systemEventInterceptor = systemEventInterceptor;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingTask = Task.Delay(500);

            if (!await KeyboardBacklightPage.IsSupportedAsync())
                _navigationStore.Items.Remove(_keyboardItem);

            ContextMenuHelper.Instance.SetNavigationItems(_navigationStore);

            await loadingTask;

            _loader.IsLoading = false;

            LoadDeviceInfo();
            CheckForUpdates();

            InitializeTray();
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (_settings.Store.MinimizeOnClose)
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

                _systemEventInterceptor = null;
                _notifyIcon?.Unregister();

                Application.Current.Shutdown();
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

        private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
            var deviceInformationWindow = new DeviceInformationWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            deviceInformationWindow.ShowDialog();
        }

        private void UpdateIndicator_Click(object sender, RoutedEventArgs e)
        {
            var updateWindow = new UpdateWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            updateWindow.ShowDialog();
        }

        private void NotifyIcon_LeftClick([NotNull] NotifyIcon sender, RoutedEventArgs e) => BringToForeground();

        private void LoadDeviceInfo()
        {
            Task.Run(Compatibility.GetMachineInformation)
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
                    }
                    else
                    {
                        _updateIndicator.Content = $"Update {result.ToString(3)} available!";
                        _updateIndicator.Visibility = Visibility.Visible;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void BringToForeground()
        {
            ShowInTaskbar = true;

            if (WindowState == WindowState.Minimized || Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }

        public void SendToTray()
        {
            Hide();
            ShowInTaskbar = false;
        }
    }
}
