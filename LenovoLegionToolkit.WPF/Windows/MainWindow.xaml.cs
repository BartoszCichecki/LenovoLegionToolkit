using System.Collections.ObjectModel;
using System.Windows;
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
    }
}
