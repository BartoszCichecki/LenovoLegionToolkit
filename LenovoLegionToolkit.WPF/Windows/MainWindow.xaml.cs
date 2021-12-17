using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Pages;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeNavigation();
        }

        private void InitializeNavigation()
        {
            RootNavigation.Frame = RootFrame;
            RootNavigation.Items = new ObservableCollection<NavItem>
{
                new() { Icon = WPFUI.Common.Icon.Home20, Name = "Dashboard", Tag = "dashboard", Type = typeof(Dashboard)},
            };
            RootNavigation.Footer = new ObservableCollection<NavItem>
            {
                new() { Icon = WPFUI.Common.Icon.Settings20, Name = "Settings", Tag = "settings", Type = typeof(SettingsPage)},
                new() { Icon = WPFUI.Common.Icon.Info20, Name = "About", Tag = "about", Type = typeof(AboutPage)},
            };

            RootNavigation.Navigate("dashboard");
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
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

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
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
            }
        }

        private void NotifyIcon_Open(object sender, RoutedEventArgs e) => BringToForeground();

        private void NotifyIcon_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

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
