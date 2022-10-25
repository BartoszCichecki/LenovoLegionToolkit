using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard
{
    public partial class GodModeSettingsWindow
    {
        private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
        private readonly GodModeController _controller = IoCContainer.Resolve<GodModeController>();

        private bool _isRefreshing;

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
            _isRefreshing = true;

            try
            {
                _loader.IsLoading = true;
                _applyRevertStackPanel.Visibility = Visibility.Hidden;

                var loadingTask = Task.Delay(500);

                var state = await _controller.GetStateAsync();

                var maxValueOffset = state.MaxValueOffset;

                SetSliderValues(_cpuLongTermPowerLimitCardControl, _cpuLongTermPowerLimitSlider, state.CPULongTermPowerLimit, maxValueOffset);
                SetSliderValues(_cpuShortTermPowerLimitCardControl, _cpuShortTermPowerLimitSlider, state.CPUShortTermPowerLimit, maxValueOffset);
                SetSliderValues(_cpuCrossLoadingLimitCardControl, _cpuCrossLoadingLimitSlider, state.CPUCrossLoadingPowerLimit, maxValueOffset);
                SetSliderValues(_cpuTemperatureLimitCardControl, _cpuTemperatureLimitSlider, state.CPUTemperatureLimit);
                SetSliderValues(_gpuPowerBoostCardControl, _gpuPowerBoostSlider, state.GPUPowerBoost, maxValueOffset);
                SetSliderValues(_gpuConfigurableTGPCardControl, _gpuConfigurableTGPSlider, state.GPUConfigurableTGP, maxValueOffset);
                SetSliderValues(_gpuTemperatureLimitCardControl, _gpuTemperatureLimitSlider, state.GPUTemperatureLimit);

                var fanTableInfo = state.FanTableInfo;
                if (fanTableInfo.HasValue)
                    _fanCurveControl.SetFanTableInfo(fanTableInfo.Value);
                else
                    _fanCurveCardControl.Visibility = Visibility.Collapsed;

                _fanCurveCardControl.IsEnabled = !state.FanFullSpeed;
                _fanFullSpeedToggle.IsChecked = state.FanFullSpeed;

                _maxValueOffsetNumberBox.Text = $"{maxValueOffset}";

                await loadingTask;

                _applyRevertStackPanel.Visibility = Visibility.Visible;
                _loader.IsLoading = false;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't load settings.", ex);

                await _snackBar.ShowAsync(Resource.GodModeSettingsWindow_Error_Load_Title, ex.Message);

                Close();
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private async Task ApplyAsync()
        {
            try
            {
                var state = await _controller.GetStateAsync();

                StepperValue? cpuLongTermPowerLimit = null;
                StepperValue? cpuShortTermPowerLimit = null;
                StepperValue? cpuCrossLoadingPowerLimit = null;
                StepperValue? cpuTemperatureLimit = null;
                StepperValue? gpuPowerBoost = null;
                StepperValue? gpuConfigurableTGP = null;
                StepperValue? gpuTemperatureLimit = null;

                if (state.CPULongTermPowerLimit.HasValue)
                    cpuLongTermPowerLimit = state.CPULongTermPowerLimit.Value.WithValue((int)_cpuLongTermPowerLimitSlider.Value);

                if (state.CPUShortTermPowerLimit.HasValue)
                    cpuShortTermPowerLimit = state.CPUShortTermPowerLimit.Value.WithValue((int)_cpuShortTermPowerLimitSlider.Value);

                if (state.CPUCrossLoadingPowerLimit.HasValue)
                    cpuCrossLoadingPowerLimit = state.CPUCrossLoadingPowerLimit.Value.WithValue((int)_cpuCrossLoadingLimitSlider.Value);

                if (state.CPUTemperatureLimit.HasValue)
                    cpuTemperatureLimit = state.CPUTemperatureLimit.Value.WithValue((int)_cpuTemperatureLimitSlider.Value);

                if (state.GPUPowerBoost.HasValue)
                    gpuPowerBoost = state.GPUPowerBoost.Value.WithValue((int)_gpuPowerBoostSlider.Value);

                if (state.GPUConfigurableTGP.HasValue)
                    gpuConfigurableTGP = state.GPUConfigurableTGP.Value.WithValue((int)_gpuConfigurableTGPSlider.Value);

                if (state.GPUTemperatureLimit.HasValue)
                    gpuTemperatureLimit = state.GPUTemperatureLimit.Value.WithValue((int)_gpuTemperatureLimitSlider.Value);

                var fanTableInfo = _fanCurveControl.GetFanTableInfo();
                var fanFullSpeed = _fanFullSpeedToggle.IsChecked ?? false;

                var maxValueOffset = (int)_maxValueOffsetNumberBox.Value;

                var newState = new GodModeState
                {
                    CPULongTermPowerLimit = cpuLongTermPowerLimit,
                    CPUShortTermPowerLimit = cpuShortTermPowerLimit,
                    CPUCrossLoadingPowerLimit = cpuCrossLoadingPowerLimit,
                    CPUTemperatureLimit = cpuTemperatureLimit,
                    GPUPowerBoost = gpuPowerBoost,
                    GPUConfigurableTGP = gpuConfigurableTGP,
                    GPUTemperatureLimit = gpuTemperatureLimit,
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
                    Log.Instance.Trace($"Couldn't apply settings", ex);

                await _snackBar.ShowAsync(Resource.GodModeSettingsWindow_Error_Apply_Title, ex.Message);
            }
        }

        private void SetSliderValues(CardControl cardControl, Slider slider, StepperValue? stepperValue, int maxValueOffset = 0)
        {
            if (!stepperValue.HasValue)
            {
                cardControl.Visibility = Visibility.Collapsed;
                return;
            }

            slider.Minimum = stepperValue.Value.Min;
            slider.Maximum = stepperValue.Value.Max + maxValueOffset;
            slider.TickFrequency = stepperValue.Value.Step;
            slider.Value = stepperValue.Value.Value;

            if (stepperValue.Value.Min == stepperValue.Value.Max + maxValueOffset)
                slider.IsEnabled = false;
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
            if (_isRefreshing)
                return;

            if (_cpuLongTermPowerLimitSlider.Value > _cpuShortTermPowerLimitSlider.Value)
                _cpuShortTermPowerLimitSlider.Value = _cpuLongTermPowerLimitSlider.Value;
        }

        private void CpuShortTermPowerLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isRefreshing)
                return;

            if (_cpuLongTermPowerLimitSlider.Value > _cpuShortTermPowerLimitSlider.Value)
                _cpuLongTermPowerLimitSlider.Value = _cpuShortTermPowerLimitSlider.Value;
        }

        private void FanFullSpeedToggle_OnClick(object sender, RoutedEventArgs e)
        {
            _fanCurveCardControl.IsEnabled = !(_fanFullSpeedToggle.IsChecked ?? false);
        }
    }
}
