using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Settings;
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
        private readonly RGBKeyboardBacklightController _rgbKeyboardBacklightController = IoCContainer.Resolve<RGBKeyboardBacklightController>();
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

            _themeComboBox.SetItems(Enum.GetValues<Theme>(), _settings.Store.Theme);
            _accentColor.SetColor(_themeManager.AccentColor);
            _accentColor.SetSyncSystemAccentColor(_settings.Store.SyncSystemAccentColor);
            _autorunToggle.IsChecked = Autorun.IsEnabled;
            _minimizeOnCloseToggle.IsChecked = _settings.Store.MinimizeOnClose;

            var vantageStatus = await _vantage.GetStatusAsync();
            _vantageCard.Visibility = vantageStatus != SoftwareStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _vantageToggle.IsChecked = vantageStatus == SoftwareStatus.Disabled;

            var fnKeysStatus = await _fnKeys.GetStatusAsync();
            _fnKeysCard.Visibility = fnKeysStatus != SoftwareStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _fnKeysToggle.IsChecked = fnKeysStatus == SoftwareStatus.Disabled;

            _dontShowNotificationsToggle.IsChecked = _settings.Store.DontShowNotifications;
            _dontShowNotificationsCard.Visibility = fnKeysStatus == SoftwareStatus.Disabled ? Visibility.Visible : Visibility.Collapsed;

            _excludeRefreshRatesCard.Visibility = fnKeysStatus == SoftwareStatus.Disabled ? Visibility.Visible : Visibility.Collapsed;

            await loadingTask;

            _themeComboBox.Visibility = Visibility.Visible;
            _autorunToggle.Visibility = Visibility.Visible;
            _minimizeOnCloseToggle.Visibility = Visibility.Visible;
            _vantageToggle.Visibility = Visibility.Visible;
            _fnKeysToggle.Visibility = Visibility.Visible;
            _dontShowNotificationsToggle.Visibility = Visibility.Visible;

            _isRefreshing = false;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (!_themeComboBox.TryGetSelectedItem(out Theme state))
                return;

            _settings.Store.Theme = state;
            _settings.SynchronizeStore();

            _themeManager.Apply();
        }

        private void AccentColorControl_OnChanged(object sender, EventArgs e)
        {
            if (_isRefreshing)
                return;

            _settings.Store.AccentColor = _accentColor.GetColor();
            _settings.SynchronizeStore();

            _themeManager.Apply();
        }

        private void AccentColorControl_OnSyncSystemAccentColorChanged(object sender, EventArgs e)
        {
            if (_isRefreshing)
                return;

            _settings.Store.SyncSystemAccentColor = _accentColor.GetSyncSystemAccentColor();
            _settings.SynchronizeStore();

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

            _settings.Store.MinimizeOnClose = state.Value;
            _settings.SynchronizeStore();
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
            {
                try
                {
                    await _vantage.DisableAsync();
                }
                catch
                {
                    await SnackbarHelper.ShowAsync("Couldn't disable Vantage", "Vantage may have not been disabled correctly", true);
                    return;
                }

                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Setting light controll owner and restoring preset...");

                    await _rgbKeyboardBacklightController.SetLightControlOwnerAsync(true, true);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Couldn't set light controll owner or current preset.", ex);
                }
            }
            else
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Setting light controll owner...");

                    await _rgbKeyboardBacklightController.SetLightControlOwnerAsync(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Couldn't set light controll owner.", ex);
                }

                try
                {
                    await _vantage.EnableAsync();
                }
                catch
                {
                    await SnackbarHelper.ShowAsync("Couldn't enable Vantage", "Vantage may have not been enabled correctly", true);
                    return;
                }
            }

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
            {
                try
                {
                    await _fnKeys.DisableAsync();
                }
                catch
                {
                    await SnackbarHelper.ShowAsync("Couldn't disnable Fn Keys", "Fn Keys may have not been disabled correctly", true);
                    return;
                }
            }
            else
            {
                try
                {
                    await _fnKeys.EnableAsync();
                }
                catch
                {
                    await SnackbarHelper.ShowAsync("Couldn't enable Fn Keys", "Fn Keys may have not been enabled correctly", true);
                    return;
                }
            }

            _fnKeysToggle.IsEnabled = true;

            _dontShowNotificationsCard.Visibility = state.Value ? Visibility.Visible : Visibility.Collapsed;
            _excludeRefreshRatesCard.Visibility = state.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DontShowNotificationsToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
                return;

            _dontShowNotificationsToggle.IsEnabled = false;

            var state = _dontShowNotificationsToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.DontShowNotifications = state.Value;
            _settings.SynchronizeStore();

            _dontShowNotificationsToggle.IsEnabled = true;
        }
        private void ExcludeRefreshRates_Click(object sender, RoutedEventArgs e)
        {
            var window = new ExcludeRefreshRatesWindow
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            window.ShowDialog();
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
