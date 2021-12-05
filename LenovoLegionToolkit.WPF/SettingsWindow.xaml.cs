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
        }

        private void SettingsWindow_Closing(object sender, CancelEventArgs e)
        {
            Save(cbQuietMode, PowerModeState.Quiet);
            Save(cbBalanceMode, PowerModeState.Balance);
            Save(cbPerformanceMode, PowerModeState.Performance);

            Settings.Instance.Synchronize();
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
