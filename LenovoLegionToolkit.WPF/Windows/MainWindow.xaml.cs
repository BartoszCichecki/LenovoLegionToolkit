using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Pages;
using LenovoLegionToolkit.WPF.Utils;
using WPFUI.Controls;
using WPFUI.Controls.Interfaces;

namespace LenovoLegionToolkit.WPF.Windows
{
    public partial class MainWindow
    {
        private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();

        private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();
        private readonly UpdateChecker _updateChecker = IoCContainer.Resolve<UpdateChecker>();

        public Snackbar Snackbar => _snackBar;

        public MainWindow()
        {
            InitializeComponent();
            InitializeNavigation();
            InitializeTray();
            RestoreWindowSize();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            IsVisibleChanged += MainWindow_IsVisibleChanged;
            StateChanged += MainWindow_StateChanged;

#if DEBUG
            _title.Text += " [DEBUG]";
#endif

            if (Log.Instance.IsTraceEnabled)
                _title.Text += " [TRACE ENABLED]";
        }

        private void InitializeNavigation()
        {
            _rootNavigation.Frame = _rootFrame;
            _rootNavigation.Items = new ObservableCollection<INavigationItem>
            {
                new NavigationItem() { Icon = WPFUI.Common.SymbolRegular.Home24, Content = "Dashboard", PageTag = "dashboard", Page = typeof(DashboardPage) },
                new NavigationItem() { Icon = WPFUI.Common.SymbolRegular.Rocket24, Content = "Actions", PageTag = "automation", Page = typeof(AutomationPage) }
            };
            _rootNavigation.Footer = new ObservableCollection<INavigationItem>
            {
                new NavigationItem() { Icon = WPFUI.Common.SymbolRegular.Settings24, Content = "Settings", PageTag = "settings", Page = typeof(SettingsPage) },
                new NavigationItem() { Icon = WPFUI.Common.SymbolRegular.Info24, Content = "About", PageTag = "about", Page = typeof(AboutPage) },
            };

            _rootNavigation.Navigate(_rootNavigation.Items[0].PageTag);
        }

        private void InitializeTray()
        {
            var openMenuItem = new MenuItem { Header = "Open" };
            openMenuItem.Click += (s, e) => BringToForeground();

            var closeMenuItem = new MenuItem { Header = "Close" };
            closeMenuItem.Click += (s, e) => Application.Current.Shutdown();

            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(openMenuItem);
            contextMenu.Items.Add(closeMenuItem);

            var notifyIcon = new NotifyIcon
            {
                TooltipText = "Lenovo Legion Toolkit",
                Icon = ImageSourceExtensions.ApplicationIcon(),
                FocusOnLeftClick = false,
                MenuOnRightClick = true,
                Menu = contextMenu,
            };
            notifyIcon.LeftClick += NotifyIcon_LeftClick;

            _titleBar.Tray = notifyIcon;

            _titleBar.Tray.Unregister();
        }

        private void RefreshAutomationMenuItems(List<AutomationPipeline> pipelines)
        {
            var contextMenu = _titleBar.Tray.Menu;
            if (contextMenu is null)
                return;

            var currentItems = contextMenu.Items.ToArray()
                .OfType<Control>()
                .Where(mi => "dynamic".Equals(mi.Tag));
            foreach (var item in currentItems)
                contextMenu.Items.Remove(item);

            var suffix = ThemeManager.IsDarkMode ? "dm" : "lm";

            var items = new List<Control>();

            var actionsItem = new MenuItem
            {
                Header = "Actions",
                Tag = "dynamic",
                IsEnabled = false,
            };
            items.Add(actionsItem);

            var menuPipelines = pipelines.Where(p => p.Triggers.Count < 1);
            foreach (var menuPipeline in menuPipelines)
            {
                var item = new MenuItem
                {
                    Header = menuPipeline.Name ?? "Unnamed flow",
                    Tag = "dynamic",
                };
                item.Click += async (s, e) => await _automationProcessor.RunNowAsync(menuPipeline);
                items.Insert(0, item);
            }

            if (items.Any())
                items.Insert(0, new Separator { Tag = "dynamic" });

            foreach (var item in items)
                contextMenu.Items.Insert(0, item);
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

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var pipelines = await _automationProcessor.GetPipelinesAsync();
            RefreshAutomationMenuItems(pipelines);

            _automationProcessor.PipelinesChanged += (s, e) => RefreshAutomationMenuItems(e.Pipelines);
            _themeManager.ThemeApplied += async (s, e) =>
            {
                var pipelines = await _automationProcessor.GetPipelinesAsync();
                RefreshAutomationMenuItems(pipelines);
            };

            CheckForUpdates();
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            SaveWindowSize();

            if (Settings.Instance.MinimizeOnClose)
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

            Settings.Instance.WindowSize = new(ActualWidth, ActualHeight);
            Settings.Instance.Synchronize();
        }

        private void RestoreWindowSize()
        {
            var windowSize = Settings.Instance.WindowSize;
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

            _titleBar.Tray.Unregister();
        }

        public void SendToTray()
        {
            _titleBar.Tray.Register();

            Hide();
            ShowInTaskbar = false;
        }
    }
}
