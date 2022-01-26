using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Dialogs;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class SettingsPage
    {
        private static readonly object DEFAULT_VALUE = new string("(Default)");

        private readonly PowerModeFeature _powerModeFeature = Container.Resolve<PowerModeFeature>();

        private bool _isRefreshing;

        public SettingsPage()
        {
            InitializeComponent();

            _autorunToggle.OnOffContent();
            _minimizeOnCloseToggle.OnOffContent();
            _vantageToggle.OnOffContent();
            _activatePowerProfilesWithVantageEnabledToggle.OnOffContent();

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

            _themeComboBox.SetItems(Enum.GetValues<Theme>(), Settings.Instance.Theme);

            _autorunToggle.IsChecked = Autorun.IsEnabled;

            _minimizeOnCloseToggle.IsChecked = Settings.Instance.MinimizeOnClose;

            var vantageStatus = await Vantage.GetStatusAsync();
            _vantageCard.Visibility = vantageStatus != VantageStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _vantageToggle.IsChecked = vantageStatus == VantageStatus.Enabled;

            var powerPlans = (await Power.GetPowerPlansAsync()).OrderBy(x => x.Name);
            Refresh(_quietModeComboBox, powerPlans, PowerModeState.Quiet);
            Refresh(_balanceModeComboBox, powerPlans, PowerModeState.Balance);
            Refresh(_performanceModeComboBox, powerPlans, PowerModeState.Performance);

            _activatePowerProfilesWithVantageEnabledToggle.IsChecked = Settings.Instance.ActivatePowerProfilesWithVantageEnabled;

            _isRefreshing = false;
        }

        private void Refresh(ComboBox comboBox, IEnumerable<PowerPlan> powerPlans, PowerModeState powerModeState)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add(DEFAULT_VALUE);
            comboBox.Items.AddRange(powerPlans);
            comboBox.SelectedValue = powerPlans.FirstOrDefault(pp => pp.InstanceID == Settings.Instance.PowerPlans.GetValueOrDefault(powerModeState)) ?? DEFAULT_VALUE;
        }

        private async Task PowerPlanChangedAsync(object value, PowerModeState powerModeState)
        {
            if (value is PowerPlan powerPlan)
                Settings.Instance.PowerPlans[powerModeState] = powerPlan.InstanceID;
            if (value is string)
                Settings.Instance.PowerPlans.Remove(powerModeState);
            Settings.Instance.Synchronize();

            await _powerModeFeature.EnsureCorrectPowerPlanIsSetAsync();
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

        private async void QuietModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            var state = _quietModeComboBox.SelectedValue;
            if (state == null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Quiet);
        }

        private async void BalanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            var state = _balanceModeComboBox.SelectedValue;
            if (state == null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Balance);
        }

        private async void PerformanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            var state = _performanceModeComboBox.SelectedValue;
            if (state == null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Performance);
        }

        private async void ActivatePowerProfilesWithVantageEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
                return;

            var state = _activatePowerProfilesWithVantageEnabledToggle.IsChecked;
            if (state == null)
                return;

            if (state.Value && !await DialogService.ShowDialogAsync(
                "Are you sure?",
                "Enabling this option when Lenovo Vantage is running and it changes power plans on your laptop might result in unexpected behavior.",
                "Yes, enable",
                "No, do not enable"))
            {
                _activatePowerProfilesWithVantageEnabledToggle.IsChecked = false;
                return;
            }

            Settings.Instance.ActivatePowerProfilesWithVantageEnabled = state.Value;
            Settings.Instance.Synchronize();

            await Container.Resolve<PowerModeFeature>().EnsureCorrectPowerPlanIsSetAsync();
        }
    }
}
