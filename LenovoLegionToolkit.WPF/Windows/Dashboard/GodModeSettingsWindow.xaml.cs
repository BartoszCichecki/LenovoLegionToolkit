using System;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard
{
    public partial class GodModeSettingsWindow
    {
        private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
        private readonly GodModeController _controller = IoCContainer.Resolve<GodModeController>();

        public GodModeSettingsWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            Loaded += GodModeSettingsWindow_Loaded;
            IsVisibleChanged += GodModeSettingsWindow_IsVisibleChanged;
        }

        private async void GodModeSettingsWindow_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private async void GodModeSettingsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                _loader.IsLoading = true;
                _applyRevertStackPanel.Visibility = Visibility.Hidden;

                var loadingTask = Task.Delay(500);

                var state = await _controller.GetStateAsync();

                var maxValueOffset = state.MaxValueOffset;

                _cpuLongTermPowerLimitSlider.Minimum = state.CPULongTermPowerLimit.Min;
                _cpuLongTermPowerLimitSlider.Maximum = state.CPULongTermPowerLimit.Max + maxValueOffset;
                _cpuLongTermPowerLimitSlider.TickFrequency = state.CPULongTermPowerLimit.Step;
                _cpuLongTermPowerLimitSlider.Value = state.CPULongTermPowerLimit.Value;

                _cpuShortTermPowerLimitSlider.Minimum = state.CPUShortTermPowerLimit.Min;
                _cpuShortTermPowerLimitSlider.Maximum = state.CPUShortTermPowerLimit.Max + maxValueOffset;
                _cpuShortTermPowerLimitSlider.TickFrequency = state.CPUShortTermPowerLimit.Step;
                _cpuShortTermPowerLimitSlider.Value = state.CPUShortTermPowerLimit.Value;

                _gpuPowerBoostSlider.Minimum = state.GPUPowerBoost.Min;
                _gpuPowerBoostSlider.Maximum = state.GPUPowerBoost.Max + maxValueOffset;
                _gpuPowerBoostSlider.TickFrequency = state.GPUPowerBoost.Step;
                _gpuPowerBoostSlider.Value = state.GPUPowerBoost.Value;

                _gpuConfigurableTGPSlider.Minimum = state.GPUConfigurableTGP.Min;
                _gpuConfigurableTGPSlider.Maximum = state.GPUConfigurableTGP.Max + maxValueOffset;
                _gpuConfigurableTGPSlider.TickFrequency = state.GPUConfigurableTGP.Step;
                _gpuConfigurableTGPSlider.Value = state.GPUConfigurableTGP.Value;

                var fanTableInfo = state.FanTableInfo;
                if (fanTableInfo.HasValue)
                    _fanCurveControl.SetFanTableInfo(fanTableInfo.Value);
                else
                    _fanCurveCardControl.Visibility = Visibility.Collapsed;

                _fanCurveCardControl.IsEnabled = !state.FanFullSpeed;
                _fanFullSpeedToggle.IsChecked = state.FanFullSpeed;

                if (state.CPULongTermPowerLimit.Min == state.CPULongTermPowerLimit.Max)
                    _cpuLongTermPowerLimitSlider.IsEnabled = false;
                if (state.CPUShortTermPowerLimit.Min == state.CPUShortTermPowerLimit.Max)
                    _cpuShortTermPowerLimitSlider.IsEnabled = false;
                if (state.GPUPowerBoost.Min == state.GPUPowerBoost.Max)
                    _gpuPowerBoostSlider.IsEnabled = false;
                if (state.GPUConfigurableTGP.Min == state.GPUConfigurableTGP.Max)
                    _gpuConfigurableTGPSlider.IsEnabled = false;

                _maxValueOffsetNumberBox.Text = $"{maxValueOffset}";

                await loadingTask;

                _applyRevertStackPanel.Visibility = Visibility.Visible;
                _loader.IsLoading = false;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't load settings.", ex);

                await _snackBar.ShowAsync("Couldn't load setting.", ex.Message);

                Close();
            }
        }

        private async Task ApplyAsync()
        {
            try
            {
                var state = await _controller.GetStateAsync();

                var cpuLongTermPowerLimit = state.CPULongTermPowerLimit.WithValue((int)_cpuLongTermPowerLimitSlider.Value);
                var cpuShortTermPowerLimit = state.CPUShortTermPowerLimit.WithValue((int)_cpuShortTermPowerLimitSlider.Value);
                var gpuPowerBoost = state.GPUPowerBoost.WithValue((int)_gpuPowerBoostSlider.Value);
                var gpuConfigurableTGP = state.GPUConfigurableTGP.WithValue((int)_gpuConfigurableTGPSlider.Value);

                var fanTableInfo = _fanCurveControl.GetFanTableInfo();
                var fanFullSpeed = _fanFullSpeedToggle.IsChecked ?? false;

                var maxValueOffset = (int)_maxValueOffsetNumberBox.Value;

                var newState = new GodModeState
                {
                    CPULongTermPowerLimit = cpuLongTermPowerLimit,
                    CPUShortTermPowerLimit = cpuShortTermPowerLimit,
                    GPUPowerBoost = gpuPowerBoost,
                    GPUConfigurableTGP = gpuConfigurableTGP,
                    FanTableInfo = fanTableInfo,
                    FanFullSpeed = fanFullSpeed,
                    MaxValueOffset = maxValueOffset,
                };

                if (await _powerModeFeature.GetStateAsync() != PowerModeState.GodMode)
                    await _powerModeFeature.SetStateAsync(PowerModeState.GodMode);

                await _controller.SetStateAsync(newState);
                await _controller.ApplyStateAsync();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't apply settings.", ex);

                await _snackBar.ShowAsync("Couldn't apply setting.", ex.Message);
            }
        }

        private async void ApplyAndCloseButton_Click(object sender, RoutedEventArgs e)
        {
            await ApplyAsync();
            Close();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            await ApplyAsync();
            await RefreshAsync();
        }

        private void CpuLongTermPowerLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_cpuLongTermPowerLimitSlider.Value > _cpuShortTermPowerLimitSlider.Value)
                _cpuShortTermPowerLimitSlider.Value = _cpuLongTermPowerLimitSlider.Value;
        }

        private void CpuShortTermPowerLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_cpuLongTermPowerLimitSlider.Value > _cpuShortTermPowerLimitSlider.Value)
                _cpuLongTermPowerLimitSlider.Value = _cpuShortTermPowerLimitSlider.Value;
        }

        private void FanFullSpeedToggle_OnClick(object sender, RoutedEventArgs e)
        {
            _fanCurveCardControl.IsEnabled = !(_fanFullSpeedToggle.IsChecked ?? false);
        }
    }
}
