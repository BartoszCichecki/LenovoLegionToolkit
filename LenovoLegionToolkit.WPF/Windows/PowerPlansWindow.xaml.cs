using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows
{
    public partial class PowerPlansWindow
    {
        private readonly PowerModeFeature _powerModeFeature = Container.Resolve<PowerModeFeature>();

        private static readonly object DEFAULT_VALUE = new string("(Default)");

        private bool _isRefreshing;

        public PowerPlansWindow()
        {
            InitializeComponent();

            Loaded += PowerPlansWindow_Loaded;
            IsVisibleChanged += PowerPlansWindow_IsVisibleChanged;
        }

        private async void PowerPlansWindow_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private async void PowerPlansWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            _isRefreshing = true;

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

            if (state.Value && !await MessageBoxHelper.ShowAsync(
                this,
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
