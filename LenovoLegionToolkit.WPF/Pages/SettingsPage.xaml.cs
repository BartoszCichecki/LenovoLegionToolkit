using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public SettingsPage()
        {
            InitializeComponent();
        }

        private async Task RefreshAsync()
        {
            _themeComboBox.SetItems(Enum.GetValues<Theme>(), Settings.Instance.Theme);

            _autorunToggleButton.IsChecked = Autorun.IsEnabled;

            _minimizeOnCloseToggleButton.IsChecked = Settings.Instance.MinimizeOnClose;

            var vantageStatus = await Vantage.GetStatusAsync();
            _vantageCard.Visibility = vantageStatus != VantageStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _vantageToggleButton.IsChecked = vantageStatus == VantageStatus.Enabled;

            var powerPlans = (await Power.GetPowerPlansAsync()).OrderBy(x => x.Name);
            Refresh(_quietModeComboBox, powerPlans, PowerModeState.Quiet);
            Refresh(_balanceModeComboBox, powerPlans, PowerModeState.Balance);
            Refresh(_performanceModeComboBox, powerPlans, PowerModeState.Performance);

            _activatePowerProfilesWithVantageEnabledToggleButton.IsChecked = Settings.Instance.ActivatePowerProfilesWithVantageEnabled;
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

        private async void SettingsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                await RefreshAsync();
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_themeComboBox.TryGetSelectedItem(out Theme state))
                return;

            Settings.Instance.Theme = state;
            Settings.Instance.Synchronize();

            Container.Resolve<ThemeManager>().Apply();
        }

        private void AutorunToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _autorunToggleButton.IsChecked;
            if (state == null)
                return;

            if (state.Value)
                Autorun.Enable();
            else
                Autorun.Disable();
        }

        private void MinimizeOnCloseToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _minimizeOnCloseToggleButton.IsChecked;
            if (state == null)
                return;

            Settings.Instance.MinimizeOnClose = state.Value;
            Settings.Instance.Synchronize();
        }

        private async void VantageToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _vantageToggleButton.IsChecked;
            if (state == null)
                return;

            if (state.Value)
                await Vantage.EnableAsync();
            else
                await Vantage.DisableAsync();
        }

        private async void QuietModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _quietModeComboBox.SelectedValue;
            if (state == null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Quiet);
        }

        private async void BalanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _balanceModeComboBox.SelectedValue;
            if (state == null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Balance);
        }

        private async void PerformanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _performanceModeComboBox.SelectedValue;
            if (state == null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Performance);
        }

        private async void ActivatePowerProfilesWithVantageEnabled_Click(object sender, RoutedEventArgs e)
        {
            var state = _activatePowerProfilesWithVantageEnabledToggleButton.IsChecked;
            if (state == null)
                return;

            if (state.Value && !await DialogService.ShowDialogAsync(
                "Are you sure?",
                "Enabling this option when Lenovo Vantage is running and it changes power plans on your laptop might result in unexpected behavior.",
                "Yes, enable",
                "No, do not enable"))
            {
                _activatePowerProfilesWithVantageEnabledToggleButton.IsChecked = false;
                return;
            }

            Settings.Instance.ActivatePowerProfilesWithVantageEnabled = state.Value;
            Settings.Instance.Synchronize();

            await Container.Resolve<PowerModeFeature>().EnsureCorrectPowerPlanIsSetAsync();
        }
    }
}
