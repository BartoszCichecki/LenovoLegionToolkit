﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Dialogs;

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

        private void Refresh()
        {
            _themeComboBox.Items.Clear();
            _themeComboBox.Items.AddEnumValues<Theme>();
            _themeComboBox.SelectedValue = Settings.Instance.Theme;

            _autorunToggleButton.IsChecked = Autorun.IsEnabled;

            var vantageStatus = Vantage.Status;
            _vantageCard.Visibility = vantageStatus != VantageStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _vantageToggleButton.IsChecked = vantageStatus == VantageStatus.Enabled;

            var powerPlans = Power.GetPowerPlans().OrderBy(x => x.Name);
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

        private void PowerPlanChanged(object value, PowerModeState powerModeState)
        {
            if (value is PowerPlan powerPlan)
                Settings.Instance.PowerPlans[powerModeState] = powerPlan.InstanceID;
            if (value is string)
                Settings.Instance.PowerPlans.Remove(powerModeState);
            Settings.Instance.Synchronize();

            _powerModeFeature.EnsureCorrectPowerPlanIsSet();
        }

        private void SettingsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                Refresh();
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _themeComboBox.SelectedValue;
            if (state == null)
                return;

            Settings.Instance.Theme = (Theme)state;
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

        private void VantageToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _vantageToggleButton.IsChecked;
            if (state == null)
                return;

            if (state.Value)
                Vantage.Enable();
            else
                Vantage.Disable();
        }

        private void QuietModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _quietModeComboBox.SelectedValue;
            if (state == null)
                return;

            PowerPlanChanged(state, PowerModeState.Quiet);
        }

        private void BalanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _balanceModeComboBox.SelectedValue;
            if (state == null)
                return;

            PowerPlanChanged(state, PowerModeState.Balance);
        }

        private void PerformanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _performanceModeComboBox.SelectedValue;
            if (state == null)
                return;

            PowerPlanChanged(state, PowerModeState.Performance);
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

            Container.Resolve<PowerModeFeature>().EnsureCorrectPowerPlanIsSet();
        }
    }
}
