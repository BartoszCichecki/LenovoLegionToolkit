using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public ObservableCollection<PowerPlan> QuietPowerPlans { get; } = new();
        public ObservableCollection<PowerPlan> BalancePowerPlans { get; } = new();
        public ObservableCollection<PowerPlan> PerformancePowerPlans { get; } = new();

        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = this;

            Closing += SettingsWindow_Closing;

            var powerPlans = Power.GetPowerPlans()
                .OrderBy(x => x.Name)
                .ToList();

            powerPlans.ForEach(QuietPowerPlans.Add);
            powerPlans.ForEach(BalancePowerPlans.Add);
            powerPlans.ForEach(PerformancePowerPlans.Add);

            Load(cbQuietMode, PowerModeState.Quiet, powerPlans);
            Load(cbBalanceMode, PowerModeState.Balance, powerPlans);
            Load(cbPerformanceMode, PowerModeState.Performance, powerPlans);

            cbActivatePowerProfilesWithVantageEnabled.IsChecked = Settings.Instance.ActivatePowerProfilesWithVantageEnabled;
        }

        private void SettingsWindow_Closing(object sender, CancelEventArgs e)
        {
            Save(cbQuietMode, PowerModeState.Quiet);
            Save(cbBalanceMode, PowerModeState.Balance);
            Save(cbPerformanceMode, PowerModeState.Performance);

            Settings.Instance.Synchronize();
        }

        private void ActivatePowerProfilesWithVantageEnabled_Click(object sender, RoutedEventArgs e)
        {
            var settings = Settings.Instance;
            if (settings.ActivatePowerProfilesWithVantageEnabled)
            {
                settings.ActivatePowerProfilesWithVantageEnabled = false;
                settings.Synchronize();
            }
            else
            {
                var result = MessageBox.Show("After enabling this option Lenovo Legion Toolkit will always switch power plans." +
                    " This option may conflict with some versions of Lenovo Vantage, leading to unexpected behavior." +
                    "\n\nOnly enable this if your version of Lenovo Vantage does not switch power plans." +
                    "\n\nAre you sure you want to enable this?",
                    "Activate power plans even when Vantage is enabled",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    settings.ActivatePowerProfilesWithVantageEnabled = true;
                    settings.Synchronize();
                }
                else
                {
                    cbActivatePowerProfilesWithVantageEnabled.IsChecked = false;
                }
            }

            cbActivatePowerProfilesWithVantageEnabled.IsChecked = settings.ActivatePowerProfilesWithVantageEnabled;
        }

        private static void Load(ComboBox comboBox, PowerModeState powerModeState, List<PowerPlan> powerPlans)
        {
            var planId = Settings.Instance.PowerPlans.GetValueOrDefault(powerModeState);
            if (planId == null)
            {
                comboBox.SelectedIndex = 0;
                return;
            }

            var powerPlan = powerPlans.FirstOrDefault(pp => pp.InstanceID.Contains(planId));
            if (powerPlan == null)
                comboBox.SelectedIndex = 0;
            else
                comboBox.SelectedItem = powerPlan;
        }

        private static void Save(ComboBox comboBox, PowerModeState powerModeState)
        {
            if (comboBox.SelectedItem is PowerPlan selectedItem)
                Settings.Instance.PowerPlans[powerModeState] = selectedItem.InstanceID;
            else
                Settings.Instance.PowerPlans.Remove(powerModeState);
        }
    }
}
