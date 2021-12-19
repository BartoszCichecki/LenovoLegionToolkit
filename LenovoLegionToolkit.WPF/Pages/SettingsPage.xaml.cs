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

            _activatePowerProfilesWithVantageEnabledToggleButton.IsChecked = Settings.Instance.ActivatePowerProfilesWithVantageEnabled;
        }

        private void SettingsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                Refresh();
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_themeComboBox.SelectedValue == null)
                return;

            var value = (Theme)_themeComboBox.SelectedValue;
            Settings.Instance.Theme = value;
            Settings.Instance.Synchronize();

            Container.Resolve<ThemeManager>().Apply();
        }

        private void AutorunToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_autorunToggleButton.IsChecked == null)
                return;

            var state = _autorunToggleButton.IsChecked;
            if (state.Value)
                Autorun.Enable();
            else
                Autorun.Disable();
        }

        private void VantageToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_vantageToggleButton.IsChecked == null)
                return;

            var state = _vantageToggleButton.IsChecked;
            if (state.Value)
                Vantage.Enable();
            else
                Vantage.Disable();
        }

        private async void ActivatePowerProfilesWithVantageEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (_activatePowerProfilesWithVantageEnabledToggleButton.IsChecked == null)
                return;

            var state = _activatePowerProfilesWithVantageEnabledToggleButton.IsChecked;

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
