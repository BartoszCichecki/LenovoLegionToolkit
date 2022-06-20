using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray;

namespace LenovoLegionToolkit.WPF.Windows
{
    public partial class MainWindow
    {
        private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();
        private readonly UpdateChecker _updateChecker = IoCContainer.Resolve<UpdateChecker>();

        public Snackbar Snackbar => _snackBar;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTray();
            RestoreWindowSize();

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
                _title.Text += " [TRACE ENABLED]";
        }

        private void InitializeTray()
        {
            ContextMenuHelper.Instance.BringToForegroundAction = BringToForeground;
            ContextMenuHelper.Instance.SetNavigationItems(_navigationStore);

            var notifyIcon = new NotifyIcon
            {
                TooltipText = "Lenovo Legion Toolkit",
                Icon = ImageSourceExtensions.ApplicationIcon(),
                FocusOnLeftClick = false,
                MenuOnRightClick = true,
                Menu = ContextMenuHelper.Instance.ContextMenu,
            };
            notifyIcon.LeftClick += NotifyIcon_LeftClick;
            _titleBar.Tray = notifyIcon;
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CheckForUpdates();
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            SaveWindowSize();

            if (_settings.MinimizeOnClose)
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

                _titleBar.Tray.Unregister();
            }
        }

        private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            CheckForUpdates();
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

        private void NotifyIcon_LeftClick([NotNull] INotifyIcon sender, RoutedEventArgs e) => BringToForeground();

        private void SaveWindowSize()
        {
            if (WindowState == WindowState.Maximized)
                return;

            _settings.WindowSize = new(ActualWidth, ActualHeight);
            _settings.Synchronize();
        }

        private void RestoreWindowSize()
        {
            var windowSize = _settings.WindowSize;
            if (windowSize.Width >= MinWidth && windowSize.Height >= MinHeight)
            {
                Width = windowSize.Width;
                Height = windowSize.Height;
            }
        }

        private void CheckForUpdates()
        {
            Task.Run(_updateChecker.Check)
                .ContinueWith(updatesAvailable =>
                {
                    _updateIndicator.Visibility = updatesAvailable.Result ? Visibility.Visible : Visibility.Collapsed;
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
