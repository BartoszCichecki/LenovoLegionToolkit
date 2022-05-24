using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class SettingsPage
    {
        private bool _isRefreshing;

        public SettingsPage()
        {
            InitializeComponent();

            Loaded += SettingsPage_Loaded;
            IsVisibleChanged += SettingsPage_IsVisibleChanged;
        }

        private async void SettingsPage_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private async void SettingsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            _isRefreshing = true;

            var loadingTask = Task.Delay(250);

            _themeComboBox.SetItems(Enum.GetValues<Theme>(), Settings.Instance.Theme);
            _autorunToggle.IsChecked = Autorun.IsEnabled;
            _minimizeOnCloseToggle.IsChecked = Settings.Instance.MinimizeOnClose;

            var vantageStatus = await Vantage.GetStatusAsync();
            _vantageCard.Visibility = vantageStatus != VantageStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _vantageToggle.IsChecked = vantageStatus == VantageStatus.Enabled;

            await loadingTask;

            _themeComboBox.Visibility = Visibility.Visible;
            _autorunToggle.Visibility = Visibility.Visible;
            _minimizeOnCloseToggle.Visibility = Visibility.Visible;
            _vantageToggle.Visibility = Visibility.Visible;

            _isRefreshing = false;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (!_themeComboBox.TryGetSelectedItem(out Theme state))
                return;

            Settings.Instance.Theme = state;
            Settings.Instance.Synchronize();

            Container.Resolve<ThemeManager>().Apply();
        }

        private void AutorunToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
                return;

            var state = _autorunToggle.IsChecked;
            if (state == null)
                return;

            if (state.Value)
                Autorun.Enable();
            else
                Autorun.Disable();
        }

        private void MinimizeOnCloseToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
                return;

            var state = _minimizeOnCloseToggle.IsChecked;
            if (state == null)
                return;

            Settings.Instance.MinimizeOnClose = state.Value;
            Settings.Instance.Synchronize();
        }

        private async void VantageToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
                return;

            var state = _vantageToggle.IsChecked;
            if (state == null)
                return;

            if (state.Value)
                await Vantage.EnableAsync();
            else
                await Vantage.DisableAsync();
        }

        private void PowerPlans_Click(object sender, RoutedEventArgs e)
        {
            var window = new PowerPlansWindow
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            window.ShowDialog();
        }

        private void CPUBoostModes_Click(object sender, RoutedEventArgs e)
        {
            var window = new CPUBoostModesWindow
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            window.ShowDialog();
        }
    }
}
