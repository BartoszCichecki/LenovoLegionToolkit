using System.Windows;
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
            _autorunToggleButton.IsChecked = Autorun.IsEnabled;

            var vantageStatus = Vantage.Status;
            _vantageCard.Visibility = vantageStatus != VantageStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _vantageToggleButton.IsChecked = vantageStatus == VantageStatus.Enabled;

            _activatePowerProfilesWithVantageEnabledToggleButton.IsChecked = Settings.Instance.ActivatePowerProfilesWithVantageEnabled;
        }

        private void SettingsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
                Refresh();
        }

        private void AutorunToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _autorunToggleButton.IsChecked;
            if (state.Value)
                Autorun.Enable();
            else
                Autorun.Disable();
        }

        private void VantageToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _vantageToggleButton.IsChecked;
            if (state.Value)
                Vantage.Enable();
            else
                Vantage.Disable();
        }

        private async void ActivatePowerProfilesWithVantageEnabled_Click(object sender, RoutedEventArgs e)
        {
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
