using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Settings
{
    public partial class PowerPlansWindow
    {
        private static readonly object DEFAULT_VALUE = new string("(Default)");

        private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
        private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

        public PowerPlansWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

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
            _loader.IsLoading = true;

            var loadingTask = Task.Delay(500);

            var powerPlans = (await Power.GetPowerPlansAsync()).OrderBy(x => x.Name);
            Refresh(_quietModeComboBox, powerPlans, PowerModeState.Quiet);
            Refresh(_balanceModeComboBox, powerPlans, PowerModeState.Balance);
            Refresh(_performanceModeComboBox, powerPlans, PowerModeState.Performance);

            var allStates = await _powerModeFeature.GetAllStatesAsync();
            if (allStates.Contains(PowerModeState.GodMode))
                Refresh(_godModeComboBox, powerPlans, PowerModeState.GodMode);
            else
                _godModeCardControl.Visibility = Visibility.Collapsed;

            _activatePowerProfilesWithVantageEnabledToggle.IsChecked = _settings.Store.ActivatePowerProfilesWithVantageEnabled;

            await loadingTask;

            _loader.IsLoading = false;
        }

        private void Refresh(ComboBox comboBox, IEnumerable<PowerPlan> powerPlans, PowerModeState powerModeState)
        {
            var settingsPowerPlanInstanceID = _settings.Store.PowerPlans.GetValueOrDefault(powerModeState);
            var selectedValue = powerPlans.FirstOrDefault(pp => pp.InstanceID == settingsPowerPlanInstanceID);

            comboBox.Items.Clear();
            comboBox.Items.Add(DEFAULT_VALUE);
            comboBox.Items.AddRange(powerPlans);
            comboBox.SelectedValue = selectedValue.Equals(default(PowerPlan)) ? DEFAULT_VALUE : selectedValue;
        }

        private async Task PowerPlanChangedAsync(object value, PowerModeState powerModeState)
        {
            if (value is PowerPlan powerPlan)
                _settings.Store.PowerPlans[powerModeState] = powerPlan.InstanceID;
            if (value is string)
                _settings.Store.PowerPlans.Remove(powerModeState);
            _settings.SynchronizeStore();

            await _powerModeFeature.EnsureCorrectPowerPlanIsSetAsync();
        }

        private async void QuietModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _quietModeComboBox.SelectedValue;
            if (state is null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Quiet);
        }

        private async void BalanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _balanceModeComboBox.SelectedValue;
            if (state is null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Balance);
        }

        private async void PerformanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _performanceModeComboBox.SelectedValue;
            if (state is null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.Performance);
        }

        private async void GodModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = _godModeComboBox.SelectedValue;
            if (state is null)
                return;

            await PowerPlanChangedAsync(state, PowerModeState.GodMode);
        }

        private async void ActivatePowerProfilesWithVantageEnabled_Click(object sender, RoutedEventArgs e)
        {
            var state = _activatePowerProfilesWithVantageEnabledToggle.IsChecked;
            if (state is null)
                return;

            if (state.Value && !await MessageBoxHelper.ShowAsync(
                this,
                Resource.PowerPlansWindow_ActivatePowerProfilesWithVantageEnabled_Confirmation_Title,
                Resource.PowerPlansWindow_ActivatePowerProfilesWithVantageEnabled_Confirmation_Message))
            {
                _activatePowerProfilesWithVantageEnabledToggle.IsChecked = false;
                return;
            }

            _settings.Store.ActivatePowerProfilesWithVantageEnabled = state.Value;
            _settings.SynchronizeStore();

            await _powerModeFeature.EnsureCorrectPowerPlanIsSetAsync();
        }
    }
}
