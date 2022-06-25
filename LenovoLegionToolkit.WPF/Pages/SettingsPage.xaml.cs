using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Settings;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class SettingsPage
    {
        private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();
        private readonly Vantage _vantage = IoCContainer.Resolve<Vantage>();
        private readonly FnKeys _fnKeys = IoCContainer.Resolve<FnKeys>();
        private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();

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

            _themeComboBox.SetItems(Enum.GetValues<Theme>(), _settings.Theme);
            _autorunToggle.IsChecked = Autorun.IsEnabled;
            _minimizeOnCloseToggle.IsChecked = _settings.MinimizeOnClose;

            var vantageStatus = await _vantage.GetStatusAsync();
            _vantageCard.Visibility = vantageStatus != SoftwareStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _vantageToggle.IsChecked = vantageStatus == SoftwareStatus.Disabled;

            var fnKeysStatus = await _fnKeys.GetStatusAsync();
            _fnKeysCard.Visibility = fnKeysStatus != SoftwareStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _fnKeysToggle.IsChecked = fnKeysStatus == SoftwareStatus.Disabled;

            await loadingTask;

            _themeComboBox.Visibility = Visibility.Visible;
            _autorunToggle.Visibility = Visibility.Visible;
            _minimizeOnCloseToggle.Visibility = Visibility.Visible;
            _vantageToggle.Visibility = Visibility.Visible;
            _fnKeysToggle.Visibility = Visibility.Visible;

            _isRefreshing = false;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (!_themeComboBox.TryGetSelectedItem(out Theme state))
                return;

            _settings.Theme = state;
            _settings.Synchronize();

            _themeManager.Apply();
        }

        private void AutorunToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
                return;

            var state = _autorunToggle.IsChecked;
            if (state is null)
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
            if (state is null)
                return;

            _settings.MinimizeOnClose = state.Value;
            _settings.Synchronize();
        }

        private async void VantageToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
                return;

            _vantageToggle.IsEnabled = false;

            var state = _vantageToggle.IsChecked;
            if (state is null)
                return;

            if (state.Value)
                await _vantage.DisableAsync();
            else
                await _vantage.EnableAsync();

            _vantageToggle.IsEnabled = true;
        }

        private async void FnKeysToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
                return;

            _fnKeysToggle.IsEnabled = false;

            var state = _fnKeysToggle.IsChecked;
            if (state is null)
                return;

            if (state.Value)
                await _fnKeys.DisableAsync();
            else
                await _fnKeys.EnableAsync();

            _fnKeysToggle.IsEnabled = true;
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
